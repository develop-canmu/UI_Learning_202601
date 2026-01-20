using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CruFramework.Addressables;
using CruFramework.Page;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.SceneManagement;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.Networking.API;
using Pjfb.Shop;
using Pjfb.Utility;
using Pjfb.Voice;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pjfb
{
    public class AppManager : SceneRootObject
    {
#if UNITY_EDITOR
        public static readonly string StartupPageEnableKey = "Pjfb_Preference_StartupPage_Enable";
        public static readonly string StartupPagePageTypeKey = "Pjfb_Preference_StartupPage_PageType";
#endif

        private static AppManager instance = null;
        /// <summary>インタンス</summary>
        public static AppManager Instance
        {
            get { return instance; }
        }

        [SerializeField]
        private UIManager uiManager = null;
        /// <summary>UIマネージャー</summary>
        public UIManager UIManager
        {
            get { return uiManager; }
        }
        
        [SerializeField]
        private WorldManager worldManager = null;
        /// <summary>ワールドマネージャー(UI以外の要素）</summary>
        public WorldManager WorldManager
        {
            get { return worldManager; }
        }
        
        [SerializeField]
        private TutorialManager tutorialManager = null;
        /// <summary>チュートリアルマネージャー</summary>
        public TutorialManager TutorialManager
        {
            get { return tutorialManager; }
        }
        
        [SerializeField]
        private TutorialCommandManager tutorialCommandManager = null;
        /// <summary>フォーカスマネージャー</summary>
        public TutorialCommandManager TutorialCommandManager
        {
            get { return tutorialCommandManager; }
        }
        
        [SerializeField]
        private AdjustInitializer adjustInitializer = null;
        /// <summary>アジャスト</summary>
        public AdjustInitializer AdjustInitializer
        {
            get { return adjustInitializer; }
        }

        /// <summary>ロディングとタッチガードの表示をしながら非同期処理</summary>
        public async UniTask LoadingActionAsync(Func<UniTask> task)
        {
            // タッチガードとローディング表示
            uiManager.System.TouchGuard.Show();
            uiManager.System.Loading.Show();
            
            await UniTask.Lazy(task);
            // シーン遷移している場合エラー出るのでNullチェック
            if(this == null)return;
            // タッチガードとローディングを非表示
            uiManager.System.TouchGuard.Hide();
            uiManager.System.Loading.Hide();
            
        }
        
        /// <summary>ロディングとタッチガードの表示をしながら非同期処理</summary>
        public async UniTask<T> LoadingActionAsync<T>(Func<UniTask<T>> task)
        {
            // タッチガードとローディング表示
            uiManager.System.TouchGuard.Show();
            uiManager.System.Loading.Show();

            T result = await UniTask.Lazy(task);
            // シーン遷移している場合エラー出るのでNullチェック
            if(this == null)return result;
            
            // タッチガードとローディングを非表示
            uiManager.System.TouchGuard.Hide();
            uiManager.System.Loading.Hide();
            
            return result;
        }
        
        public void BackToTitle()
        {
            // エラーハンドリングイベント削除
            Application.logMessageReceived -= AppErrorHandler.LogMessageReceived;
            
            // 解放
            BGMManager.Release();
            SEManager.Release();
            VoiceManager.Instance.ReleaseAllAsset();
            DeckUtility.ClearDeckData();
            
            // キャッシュをクリア
            PlayerEnhanceManager.ClearCache();
            ShopManager.ClearCache();
            
            //Reset Dynamic Font Atlas
            DynamicAssetFontUtility.Instance.ResetFontData();
            // メモリ解放
            ReleaseMemory();
            // シーン読み直し
            SceneManager.LoadScene(SceneType.AppScene);
        }
        
        public void ReleaseMemory()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        
        /// <summary>アカウントデータ削除</summary>
        public void DeleteUserAccount()
        {
            // 不変データ削除
            Storage.LocalSaveManager.Instance.DeleteImmutableData();
            // 設定データ削除
            Storage.LocalSaveManager.Instance.DeleteData();
            //コンフィグリセット
            Menu.AppConfig.ResetConfig();
            // ショップの保持データをリセット
            Shop.ShopManager.ResetStaticVariable();
            Shop.ShopManager.ClearCache();
            // スキルコネクトのデータをリセット
            CombinationManager.ClearCache();
            CombinationManager.ClearApiCache();
            CombinationManager.ClearSavedDataCache();
            // プレイヤー強化データをリセット
            PlayerEnhanceManager.ClearCache();
            // ユーザーデータをリセット
            UserData.UserDataManager.Instance.Release();
            APIManager.Instance.setting.loginId = string.Empty;
            APIManager.Instance.setting.sessionId = string.Empty;
            // ローカルPUSHの登録削除
            LocalPushNotificationUtility.AllClear();
            // プッシュ購読の削除
            FCMPushNotificationUtility.UnregisterFirebaseMessageCallback();
        }

        private async void Awake()
        {
            instance = this;
            // アプリケーションの初期化
            await AppInitializer.Initialize();
            
            PageType startupPage = PageType.Title;
#if UNITY_EDITOR
            if(Boolean.TryParse(EditorUserSettings.GetConfigValue(StartupPageEnableKey), out bool enable))
            {
                // 起動シーンの変更が有効
                if(enable)
                {
                    if(Enum.TryParse(EditorUserSettings.GetConfigValue(StartupPagePageTypeKey), out PageType page))
                    {
                        // 起動シーン変更
                        startupPage = page;
                    }
                }
            } 
#endif
            // 初期ページへ
            UIManager.PageManager.OpenPage(startupPage, false, null);
        }
        
        private void Update()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickAndroidBackKey();
            }
#endif // UNITY_EDITOR || UNITY_ANDROID
        }
        
#if UNITY_EDITOR || UNITY_ANDROID

        /// <summary>バックキーが押下された</summary>
        private void OnClickAndroidBackKey()
        {
            ModalWindow modalWindow = (ModalWindow)uiManager.ErrorModalManager.GetTopModalWindow();
            // エラーモーダルが開かれてる場合は何もしない
            if(modalWindow != null) return;

            // TODO PageとModalManagerにステートを用意する 
            modalWindow = (ModalWindow)uiManager.ModalManager.GetTopModalWindow();
            if(modalWindow != null)
            {
                // バックキーオブジェクトが指定されてない
                if(modalWindow.BackKeyObject == null || modalWindow.DisableBackKey) return;
                OnClickAndroidBackKey(modalWindow.BackKeyObject);
                return;
            }
            
            // タイトルとホーム画面の場合はアプリを終了させる
            // TODO 画面外のボタンにレイキャストが飛ばない
            // TODO 考慮点が多いので不要なら対応なしにしたい
            // if(uiManager.PageManager.CurrentPageType == PageType.Title || uiManager.PageManager.CurrentPageType == PageType.Home)
            // {
            // }
            
            // ページオブジェクトを取得
            Pjfb.Page page = uiManager.PageManager.GetCurrentPageRecursively();
            if(page != null)
            {
                // バックキーオブジェクトが指定されてない
                if(page.BackKeyObject == null) return;
                OnClickAndroidBackKey(page.BackKeyObject);
            }
        }
        
        /// <summary>バックキーが押下された</summary>
        private void OnClickAndroidBackKey(RectTransform backKeyObject)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            
            // レイを飛ばす位置を指定
            pointerEventData.position = backKeyObject.GetScreenRect(uiManager.RootCanvas).center;
            // レイキャスト実行
            EventSystem.current.RaycastAll(pointerEventData, results);
            
            if(results.Count <= 0) return;
            
            // 一番手前のUIのイベントを発火
            ExecuteEvents.ExecuteHierarchy(results[0].gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
        }
        
        /// <summary>アプリケーションを終了する</summary>
        private void ApplicationQuit()
        {
            // アプリケーション終了確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["common.confirm"];
            // メッセージ
            data.Message = StringValueAssetLoader.Instance["application.quit_message"];
            // OKボタン
            data.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.ok"],
                window => Application.Quit()
            );
            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.cancel"], 
                window => window.Close()
            );
            // モーダル開く
            uiManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
#endif // UNITY_EDITOR || UNITY_ANDROID

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
