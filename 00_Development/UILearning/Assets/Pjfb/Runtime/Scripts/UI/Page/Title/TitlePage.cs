using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;
using UnityEngine.AddressableAssets;
using System;
using CruFramework;
using CruFramework.Addressables;
using CruFramework.Engine.CrashLog;
using CruFramework.Utils;
using Pjfb.Home;
using Pjfb.InGame;
using Pjfb.InGame.ClubRoyal;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.Story;
using Pjfb.Utility;
using Pjfb.Voice;
using UniRx;
using UnityEngine.Purchasing;
using Random = UnityEngine.Random;

namespace Pjfb.Title
{
    public class TitlePage : Page
    {
        private static readonly string AppVersionStringKey = "title.app_version";
        private static readonly string TitleCallLabelKey = "title_call";

        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private TextMeshProUGUI appVersionText = null;

        [SerializeField]
        private SplashScreen splashScreen = null;
        public SplashScreen SplashScreen{get => splashScreen;}

        private SplashScreen splashScreenInstance = null;
        private OpMovie opMovieInstance = null;

        private bool iapDone;
        private CancellationTokenSource iapTokenSource = null;

        private async UniTaskVoid OnOpenedAsync()
        {
            CancellationToken tokenOnDestroy = this.GetCancellationTokenOnDestroy();
            
            StoryManager.Instance.shownStoryHuntEnemyContainer.Clear();
            
#if !UNITY_EDITOR
            // スプラッシュスクリーン生成
            splashScreenInstance = GameObject.Instantiate<SplashScreen>(splashScreen, AppManager.Instance.UIManager.SplashScreenParent);
            // スプラッシュスクリーン再生
            UniTask playSplashScreenTask = PlaySplashScreenAsync(tokenOnDestroy);
            // OP読み込み
            UniTask loadOpMovieTask = LoadOpMovieAsync(tokenOnDestroy);
            // タスクの並列待機
            await UniTask.WhenAll(playSplashScreenTask, loadOpMovieTask);
            // OPをアクティブにする
            opMovieInstance.gameObject.SetActive(true);
            // スプラッシュ非表示
            splashScreenInstance.gameObject.SetActive(false);
            // // スプラッシュスクリーンを非表示にする
            // await splashScreenInstance.HideAsync(tokenOnDestroy);
#endif
            // ヘッダーは表示しない
            AppManager.Instance.UIManager.Header.Hide();
            // フッターは表示しない
            AppManager.Instance.UIManager.Footer.Hide();
            // ルートオブジェクト表示
            root.gameObject.SetActive(true);
            
            //Training 3D root表示
            Camera3DUtility.Instance.InitRT();
            Camera3DUtility.Instance.SetEnable3DTraningRoot(true);
#if !UNITY_EDITOR
            // OP再生
            await PlayOpMovieAsync(tokenOnDestroy);
#endif

            // エラーハンドリングイベント追加
            Application.logMessageReceived += AppErrorHandler.LogMessageReceived;

            // BGM再生
            BGMManager.PlayBGMAsync(BGM.bgm_title, true).Forget();
            // タイトルコール再生
            PlayTitleCall().Forget();
            // バージョンによる接続先決定
            await GetAppVersionAPI();

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            // 現在のデバック項目削除
            CruFramework.DebugMenu.RemoveOption(DebugMenu.DebugMenuGeneral.Category);
            // デバッグ項目追加
            DebugMenu.DebugMenuGeneral.AddOptions();
            
            // 前回起動時に応じてPolyQA起動
            if (PlayerPrefs.GetInt("EnablePolyQA", 0) == 1)
            {
                PolyQA.DataSender.LaunchManual();
            }
            
#endif
            // アジャスト初期化
            AppManager.Instance.AdjustInitializer.InitializeAdjust();
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            // アプリバージョン
            appVersionText.text = string.Format(StringValueAssetLoader.Instance[AppVersionStringKey], Application.version);
#if !PJFB_REL
            var buildNumber = Resources.Load<TextAsset>("buildnumber");
            if (buildNumber != null)
            {
                var number = buildNumber.text.Replace("\n", "");
                appVersionText.text += $"({number})";
            }
#endif
            // 演出再生
            OnOpenedAsync().Forget();
        }

        protected override void OnClosed()
        {
            GameObject.Destroy(splashScreenInstance?.gameObject);
            GameObject.Destroy(opMovieInstance?.gameObject);
        }

        /// <summary>スプラッシュスクリーン再生</summary>
        private async UniTask PlaySplashScreenAsync(CancellationToken token)
        {
            await splashScreenInstance.PlayAsync(token);
        }

        /// <summary>OP読み込み</summary>
        private async UniTask LoadOpMovieAsync(CancellationToken token)
        {
            await LoadAssetAsync<GameObject>("Prefabs/UI/Page/Title/OpMovie.prefab",
                go =>
                {
                    // OP動画生成
                    opMovieInstance = GameObject.Instantiate<GameObject>(go, AppManager.Instance.UIManager.SplashScreenParent).GetComponent<OpMovie>();
                    // 非表示
                    opMovieInstance.gameObject.SetActive(false);
                    // スプラッシュより奥に表示
                    opMovieInstance.transform.SetSiblingIndex(splashScreenInstance.transform.GetSiblingIndex());
                },
                token
            );
        }

        /// <summary>OP再生</summary>
        private async UniTask PlayOpMovieAsync(CancellationToken token)
        {
            await opMovieInstance.PlayAsync(token);
        }

        /// <summary>タイトルコール再生</summary>
        private async UniTask PlayTitleCall()
        {
            if (string.IsNullOrEmpty(LocalSaveManager.immutableData.appToken))
            {
                await LoadAssetAsync<AudioClip>("sys_0001_0001.ogg", clip => VoiceManager.Instance.PlayVoice(clip));
            }
            else
            {
                // ラベルからタイトルコールのアドレス一覧取得
                List<string> titleCallAddressList = await AddressablesManager.GetResourceLocationPrimaryKeyList(TitleCallLabelKey);
                // ランダムで再生
                int index = UnityEngine.Random.Range(0, titleCallAddressList.Count);

                // クリップ取得して再生
                await LoadAssetAsync<AudioClip>(titleCallAddressList[index], clip => VoiceManager.Instance.PlayVoice(clip), this.GetCancellationTokenOnDestroy());
            }
        }

        /// <summary>ログイン</summary>
        private async UniTaskVoid LoginAsync(CancellationToken token)
        {
            //ログイン処理前にユーザーデータをリセットする
            Pjfb.UserData.UserDataManager.Instance.Release();
            
            // 利用規約モーダル表示
            bool agreed = await TermsModal.OpenAsync(TermsModal.DisplayType.AskForAgree, token);
            // 利用規約に同意した
            if (agreed)
            {
                VersionGetVariousVerAPIResponse versionInfo = null;
                bool catalogDlSuccess = false;
                await AppManager.Instance.LoadingActionAsync(async () =>
                {
                    //appTokenがなければユーザー作成
                    if (string.IsNullOrEmpty(LocalSaveManager.immutableData.appToken))
                    {
                        await CreateUserAPI();
                        LocalSaveManager.saveData.isTermsAgreed = true;
                        LocalSaveManager.Instance.SaveData();
                    }
                    else
                    {
                        // ログイン
                        await LoginUserAPI();
                    }

                    // 利用規約が変更されたので同意しなおす
                    if (!LocalSaveManager.saveData.isTermsAgreed)
                    {
                        TermsAgreeWithLoginAPIRequest request = new TermsAgreeWithLoginAPIRequest();
                        await APIManager.Instance.Connect(request);
                        LocalSaveManager.saveData.isTermsAgreed = true;
                        LocalSaveManager.Instance.SaveData();
                    }

                    // バージョン更新
                    versionInfo = await GetVersions();

                    // カタログアップデート
                    catalogDlSuccess= await AddressablesUtility.UpdateContentCatalogAsync(token);

                    // 失敗した場合はタイトルに戻るので処理しない
                    if (!catalogDlSuccess) return;

                    //APIで使用するアセットバージョンを更新する
                    APIManager.Instance.UpdateAssetVersionWithSettingLatestVersion();
                });

                // 失敗した場合はタイトルに戻るので処理しない
                if (!catalogDlSuccess) return;

                // Tipsページデータ
                TipsPageArgs tipsArgs = new TipsPageArgs();
                // スタックする
                tipsArgs.Stack = true;

                // Tipsページで遷移前に処理するタスク
                tipsArgs.PreTask = async () =>
                {
                    // チュートリアルデータ取得のため先にマスタとユーザーデータを取得
                    await AppManager.Instance.LoadingActionAsync(async () =>
                    {
                        // マスタデータダウンロード
                        await DownloadMaster(versionInfo);
                        // ユーザーデータ取得
                        await GetUserDataAPI();
                        // マスター処理後にGC呼び出し
                        GC.Collect();
                    });

                    // チュートリアル初期化
                    await AppManager.Instance.TutorialManager.InitializeManagerAsync();
                    // スキップ確認モーダル
                    await AppManager.Instance.TutorialManager.ConfirmSkipTutorial();
                    // チュートリアルが完了しているか
                    bool isCompletedTutorial = AppManager.Instance.TutorialManager.IsCompleteTutorial();
                    // チュートリアルスキップを選択したか
                    bool isSkippedTutorial = AppManager.Instance.TutorialManager.IsSkippedTutorial();
                    
                    string downloadKey;
                    if (isSkippedTutorial || isCompletedTutorial)
                    {
                        // チュートリアルスキップ or チュートリアル完了済み
                        downloadKey = AddressablesUtility.RemotePreLoadBundleKey;
                    }
                    else
                    {
                        // チュートリアル未完了
                        downloadKey = AddressablesUtility.RemoteTutorialBundleKey;
                    }
                    
                    CancellationToken taskToken = AppManager.Instance.UIManager.PageManager.CurrentPageObject.destroyCancellationToken;
                    
                    // ダウンロード情報取得
                    AddressablesManager.DownloadInfo downloadInfo = await AddressablesUtility.GetDownloadInfoAsync(downloadKey, taskToken);
                    
                    // 必要なアセットをダウンロード
                    bool assetDownloadResult = await AddressablesUtility.EssentialAssetDownloadAsync(downloadInfo, taskToken);
                    // 失敗した場合はタイトルに戻るので以降処理しない
                    if (assetDownloadResult == false) return;
                    
                    // ヘッダーの更新イベントを登録しておく
                    AppManager.Instance.UIManager.Header.SubscribeUpdateEvents();

                    // 画像キャッシュの確認&削除
                    WebTextureManager.Release();

                    //Update CrashLog settings
                    if (StringValueAssetLoader.Instance.HasKey("crashlog.settings"))
                    {
                        CruCrashLog.LoadCrashLogRuntimeDataFromJSON(StringValueAssetLoader.Instance["crashlog.settings"]);    
                    }

                    await AppManager.Instance.LoadingActionAsync(async () =>
                    {
                        // Pending課金処理
                        await InitializeIAPController();
                        // チュートリアル関連のウォームアップ処理
                        await AppManager.Instance.TutorialManager.WarmUpShaderAsync();
                        
                        // チュートリアル完了時のみここでFCMプッシュ通知初期化（スキップ時は最後の手順だけ踏む）
                        if (isCompletedTutorial)
                        {
                            await FCMPushNotificationUtility.InitializePushStateAsync(taskToken);
                        }
                    });

                    CruCrashLog.SetUser(UserData.UserDataManager.Instance.user.uMasterId.ToString(), APIManager.Instance.setting.loginId);

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
                    // ユーザーIDをクリップボードにコピー
                    CruFramework.DebugMenu.AddOption(DebugMenu.DebugMenuGeneral.Category, $"Copy UserID : {UserData.UserDataManager.Instance.user.uMasterId}", () => GUIUtility.systemCopyBuffer = UserData.UserDataManager.Instance.user.uMasterId.ToString());
                    if (string.IsNullOrEmpty(PjfbGameHubClient.DebugAddr))
                    {
                        CruFramework.DebugMenu.RemoveOption(ClubRoyalInGamePageType.BattlePage.ToString());
                    }
#endif

                    // チュートリアルが終わってるかどうかで分岐
                    if (AppManager.Instance.TutorialManager.IsCompleteTutorial())
                    {
                        tipsArgs.PageType = PageType.Home;
                        tipsArgs.Args = new HomePage.PageParameters { isFromTitle = true };
                    }
                    else
                    {
                        TutorialSettings.Detail data = AppManager.Instance.TutorialManager.InitializeTutorialParams();
                        tipsArgs.PageType = data.TutorialPageType;
                        tipsArgs.Args = data;
                    }
                };
                
                // Tipsページへ遷移
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Tips, false, tipsArgs);
            }
        }
        
        /// <summary>バージョン情報取得</summary>
        public static async UniTask<VersionGetVariousVerAPIResponse> GetVersions()
        {
            var request = new VersionGetVariousVerAPIRequest();
            var post = new VersionGetVariousVerAPIPost();
            post.currentMasterVer = Pjfb.Storage.LocalSaveManager.saveData.masterVersion;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var responseData = request.GetResponseData();

            return responseData;
        }

        /// <summary>アプリバージョン取得</summary>
        private async UniTask GetAppVersionAPI()
        {
            VersionGetAppVerAPIRequest versionRequest = new VersionGetAppVerAPIRequest();
            //URLを設定し直す
            APIManager.Instance.setting.baseURL = APIManager.Instance.setting.versionDownloadURL;
            await APIManager.Instance.Connect(versionRequest);
        }

        /// <summary>マスターデータダウンロード</summary>
        private async UniTask DownloadMaster(VersionGetVariousVerAPIResponse versionInfo)
        {
            try
            {
                //master更新
                await Master.MasterManager.Instance.DownloadMaster(versionInfo);
            }
            catch (APIException e)
            {
                //APIエラーの場合はAPI部分がハンドリングするので何もしない
                throw e;
            }
            catch (System.Exception e)
            {
                ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.error"],
                StringValueAssetLoader.Instance["common.failed_download"],
                    null,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], window =>
                    {
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                        window.Close();
                        AppManager.Instance.BackToTitle();
                    })
                );
                AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, data);
                CruFramework.Logger.LogError("[Master] " + e.Message);
                throw new System.OperationCanceledException(e.Message);
            }

        }

        /// <summary>ユーザー生成</summary>
        private async UniTask CreateUserAPI()
        {
            UserCreateAPIRequest createRequest = new UserCreateAPIRequest();
            UserCreateAPIPost createPost = new UserCreateAPIPost();
            createPost.name = "testUser";
            createPost.gender = 1;
            createPost.deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo();
            createPost.ignoreLogin = 0;
            createPost.lastTermsAgreementAt = AppTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            createRequest.SetPostData(createPost);
            await APIManager.Instance.Connect(createRequest);
            UserCreateAPIResponse createResponse = createRequest.GetResponseData();
            APIManager.Instance.setting.sessionId = createResponse.sessionId;
            APIManager.Instance.setting.loginId = createResponse.loginId;
            AdjustManager.TrackEvent(AdjustManager.TrackEventType.UserCreate);
        }

        /// <summary>ログイン</summary>
        private async UniTask LoginUserAPI()
        {
            //ログイン
            UserLoginAPIRequest loginRequest = new UserLoginAPIRequest();
            UserLoginAPIPost loginPost = new UserLoginAPIPost();
            loginPost.appToken = LocalSaveManager.immutableData.appToken;
            loginRequest.SetPostData(loginPost);
            await APIManager.Instance.Connect(loginRequest);
        }

        /// <summary>ユーザーデータ取得</summary>
        private async UniTask GetUserDataAPI()
        {
            //ユーザーデータ取得
            var getRequest = new UserGetDataAPIRequest();
            await APIManager.Instance.Connect(getRequest);
            var getResponse = getRequest.GetResponseData();
            // 利用規約が更新された
            if (!getResponse.user.isTermsAgreed)
            {
                ConfirmModalData data = new ConfirmModalData();
                data.Title = StringValueAssetLoader.Instance["common.confirm"];
                data.Message = StringValueAssetLoader.Instance["title.terms_change_message"];
                data.NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], window => window.Close());
                var modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
                await modal.WaitCloseAsync();
                // タイトルへ
                AppManager.Instance.BackToTitle();
            }

            // Need UserGetDataAPIRequest Response Data
            AntiCheatObject.IsActive = true;
            AppGuardObject.IsActive = true;

        }

        private async UniTask InitializeIAPController()
        {
            iapDone = false;
            CancelIapToken();
            iapTokenSource = new CancellationTokenSource();
            IAPController.Instance.InitializeIapController(OnSuccessInitializeIAPController, OnFailedInitializeIAPController);

            // UnityPurchasing.Initialize結果で進めるか進めないかが左右されてしまうから最低限そこで止まることはないようにfailsafe
            // https://docs.unity3d.com/ja/2018.4/Manual/UnityIAPInitialization.html
            // とくにユーザーがデバイスを機内モードにしている場合は無期限に時間がかかります。
            if (iapTokenSource != null)
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: iapTokenSource.Token);
                    OnFailedInitializeIAPController(InitializationFailureReason.PurchasingUnavailable);
                }
                catch (OperationCanceledException) { }
            }

            await new WaitUntil(() => iapDone);
        }

        private void OnSuccessInitializeIAPController(Product[] availableProducts)
        {
            CancelIapToken();

            var pendingProduct = IAPController.Instance.GetFirstPendingProduct();
            if (pendingProduct != null)
            {
                AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"],
                    StringValueAssetLoader.Instance["iap.pending_exist"],
                     string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        window =>
                        {
                            iapDone = true;
                            window.Close();
                        })));
                return;
            }

            iapDone = true;
        }

        private void OnFailedInitializeIAPController(InitializationFailureReason reason)
        {
            CancelIapToken();

            var message = string.Empty;
            switch (reason)
            {
                case InitializationFailureReason.PurchasingUnavailable:
                    message = "PurchasingUnavailable";
                    break;
                case InitializationFailureReason.AppNotKnown:
                    message = "AppNotKnown";
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    message = "NoProductsAvailable";
                    break;
            }

            /*
            プロジェクトキーと商品登録するまでエラーが発生するのでポップアップ表示をコメントにしておく

            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"], 
                StringValueAssetLoader.Instance["iap.initialize_fail_error"], 
                message, 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], 
                window => {
                    window.Close();
                    IapDone = true;
                })));
            */

            // 本来はポップアップ閉じてから進むべきですが、一旦進むようにしている
            iapDone = true;
        }

        private void CancelIapToken()
        {
            if (iapTokenSource != null)
            {
                iapTokenSource.Cancel();
                iapTokenSource.Dispose();
                iapTokenSource = null;
            }
        }
        

        /// <summary>
        /// uGUI
        /// メニューモーダルを開く
        /// </summary>
        public void OnClickMenuButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TitleMenu, null);
        }

        /// <summary>
        /// uGUI
        /// TapToStart
        /// </summary>
        public void OnClickScreen()
        {
            LoginAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
