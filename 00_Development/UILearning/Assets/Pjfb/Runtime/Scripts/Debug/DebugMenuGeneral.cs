#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CruFramework.Addressables;
using Cysharp.Threading.Tasks;
using Pjfb.DebugPage;
using Pjfb.Master;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.SRDebugger;
using UnityEngine;
using Pjfb.UserData;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Pjfb.DebugMenu
{
    /// <summary>ゲーム全体を通して使用するデバッグ項目</summary>
    public static class DebugMenuGeneral
    {
        public static readonly string Category = "General";
        
        private static bool isForceDeleteLocalMaster = false;
        public static bool IsForceDeleteLocalMaster => isForceDeleteLocalMaster;
        
        /// <summary>ローカルプッシュ通知テスト用の秒数</summary>
        private static int localPushTestSeconds = 10;
        
        /// <summary>オプション追加</summary>
        public static void AddOptions()
        {
            // 一括追加
            foreach (DebugPageType pageType in System.Enum.GetValues(typeof(DebugPageType)))
            {
                CruFramework.DebugMenu.AddOption(Category, $"Open Debug{pageType}", () =>
                {
                    AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Debug, false, new DebugPage.DebugPage.Arguments(pageType)).Forget();
                });
            }
            
            CruFramework.DebugMenu.AddOption(Category, "デバッグ表示切り替え", () =>
            {
                GameObject go = AppManager.Instance.UIManager.RootCanvas.transform.Find("DebugPanel").gameObject;
                go.SetActive(!go.activeSelf);
            });
            
            CruFramework.DebugMenu.AddOption(Category, "強制アセットバージョン",
                () => { return PlayerPrefs.GetString("ForceAssetVersion", string.Empty); },
                (value) =>
                {
                    PlayerPrefs.SetString("ForceAssetVersion", value);
                }, 
                1);

            CruFramework.DebugMenu.AddOption(Category, "分割バンドルサイズ",
                () => { return PlayerPrefs.GetInt("bundleChunkSize", 0); },
                (value) =>
                {
                    PlayerPrefs.SetInt("bundleChunkSize", value);
                }, 
                1);
            
            SRDebugDropDownEnumData dropDownEnumData = new SRDebugDropDownEnumData(AppEnvironment.CurrentEnvironment, new []{(int)AppEnvironment.EnvironmentEnum.PROD, (int)AppEnvironment.EnvironmentEnum.COP}); 
            // 環境切り替えデバッグ機能
            CruFramework.DebugMenu.AddOption(Category, "環境切り替え",
                () => { return dropDownEnumData;},
                (enumData) =>
                {
                    AppEnvironment.EnvironmentEnum env = (AppEnvironment.EnvironmentEnum)enumData.Value;
                    if (env == AppEnvironment.EnvironmentEnum.PROD)
                    {
                        CruFramework.Logger.LogError("本番環境には変更できません");
                        return;
                    }
                    
                    // SRDebugパネルを閉じる
                    SRDebug.Instance.HideDebugPanel();
                    
                    // 環境切り替え確認モーダルの表示(static変数の初期化のために一度アプリを終了する旨を伝える)
                    ConfirmModalData modalData = new ConfirmModalData(
                        "環境切り替え",
                        $"{env}環境に切り替えるためにアプリを終了します\nよろしいですか？",
                        string.Empty,
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                            modal =>
                            {
                                // ローカルマスター削除
                                MasterManager.Instance.DeleteMaster();
                                // 環境保存
                                AppEnvironment.SaveCurrentEnv(env);
                                modal.Close();
                                // アプリ終了
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
                            }),
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                            modal =>
                            {
                                modal.Close();
                            }));
                    
                    ConfirmModalWindow.Open(modalData);
                });
            
            // ローカルマスタの削除機能
            CruFramework.DebugMenu.AddOption(Category, "ローカルマスタ削除", () =>
            {
                MasterManager.Instance.DeleteMaster();
                // 使う側から削除されたことが分かりにくいので、モーダルを出す
                ConfirmModalData modalData = new ConfirmModalData(
                    "ローカルマスタ削除",
                    $"ローカルマスタを削除しました",
                    string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        modal =>
                        {
                            modal.Close();
                        }));
                ConfirmModalWindow.Open(modalData);
            });
            
            CruFramework.DebugMenu.AddOption(Category, "マスタ強制削除", () => isForceDeleteLocalMaster, value => isForceDeleteLocalMaster = value);
            
            CruFramework.DebugMenu.AddOption(Category, "WebTextureのキャッシュ削除", () =>
            {
                WebTextureManager.ClearCache();
            });
            
            CruFramework.DebugMenu.AddOption(Category, "PolyQA切替", 
                () => PlayerPrefs.GetInt("EnablePolyQA", 0) == 1, 
                value =>
                {
                    PlayerPrefs.SetInt("EnablePolyQA", value ? 1 : 0);
                    
                    string status = value ? "有効化" : "無効化";
                    // 有効にした場合は即起動
                    if (value)
                    {
                        PolyQA.DataSender.LaunchManual();
                    }
                    
                    // モーダルを表示して閉じる
                    ConfirmModalData modalData = new ConfirmModalData(
                        "PolyQA切替",
                        $"PolyQAを {status} しました\n※ 無効化の場合はアプリを再起動してください",
                        string.Empty,
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                            modal =>
                            {
                                modal.Close();
                            }));
                    
                    ConfirmModalWindow.Open(modalData);
                    SRDebug.Instance.HideDebugPanel();
                });

            // Addressable利用するか(Unity側のFastLoad用)
            bool hasAddressable = AddressablesManager.UseAddressable;
            if (hasAddressable)
            {
                // Addressableログのアクティブ切り替え
                CruFramework.DebugMenu.AddOption(Category, "バンドルのダウンロードログ出力",
                    () => PlayerPrefs.GetInt("showBundleDownLoadInfo", 0) == 1,
                    value =>
                    {
                        PlayerPrefs.SetInt("showBundleDownLoadInfo", value ? 1 : 0);
                    });
                // バンドルキャッシュの確認
                CruFramework.DebugMenu.AddOption(Category, "アセットバンドルのキャッシュファイル確認", AddressablesUtility.ShowExistOldCacheBundle);
            }
            
            // ローカルプッシュ通知テスト
            CruFramework.DebugMenu.AddOption(Category, "ローカルPush待機時間(s)",
                () => localPushTestSeconds,
                (value) =>
                {
                    localPushTestSeconds = value;
                });
            
            CruFramework.DebugMenu.AddOption(Category, "ローカルPush送信", () =>
            {
                // ※権限ある前提の実装
                // 通知を登録
                LocalPushNotificationUtility.AddSchedule(
                    "デバッグ通知",
                    $"テスト通知です（{localPushTestSeconds}秒後）",
                    // バッジ数は1固定
                    1,
                    localPushTestSeconds
                );
                
                // 確認モーダル表示
                ConfirmModalData modalData = new ConfirmModalData(
                    "ローカルPush通知テスト",
                    $"{localPushTestSeconds}秒後に通知が届きます",
                    string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        modal =>
                        {
                            modal.Close();
                        }));
                ConfirmModalWindow.Open(modalData);
                SRDebug.Instance.HideDebugPanel();
            });
            
            CruFramework.DebugMenu.AddOption(Category, "Firebaseの初期化遅延(ms)",
                () =>
                {
                    // AppInitializerと揃えてデフォルト値を設定
#if UNITY_IOS
                    return PlayerPrefs.GetInt("firebaseInit", 100);
#else
                    return PlayerPrefs.GetInt("firebaseInit", 0);
#endif
                },
                value => PlayerPrefs.SetInt("firebaseInit", value)
                );
        }
    }
}

#endif