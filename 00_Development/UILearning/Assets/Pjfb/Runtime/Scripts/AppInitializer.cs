using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using CruFramework;
using CruFramework.Addressables;
using CruFramework.Engine.CrashLog;
using CruFramework.Utils;
using Firebase;
using Firebase.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.Utility;
using TMPro;

namespace Pjfb
{
    public static class AppInitializer
    {
        private static bool isInitialized = false;
        /// <summary>初期化済み</summary>
        public static bool IsInitialized { get { return isInitialized; } }

        public async static UniTask Initialize()
        {
            if (isInitialized) return;
            
            // マルチタッチの初期値は無効にしておく
            Input.multiTouchEnabled = false;
            
            // 60FPSにする
            QualitySettings.vSyncCount = 0;
            
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            
            // API初期化
            APIManager.Instance.SetSetting( new PjfbAPISetting() );

            APIManager.Instance.SetHandler( new PjfbAPIHandler() );
            APIManager.Instance.SetErrorHandler( new PjfbAPIErrorHandler() );
            APIManager.Instance.setting.baseURL = APIManager.Instance.setting.versionDownloadURL;

            // アドレッサブル初期化
            await AddressablesManager.InitializeAsync();
            // エラーハンドリング
            AddressablesManager.OnCompleteEvent += errorCount => 
            {
				for(int i = 0; i < errorCount; i++)
				{
					AppManager.Instance.UIManager.System.AddressableTouchGuard.Hide();
                    AppManager.Instance.UIManager.System.Loading.Hide();
				}
            };
            AddressablesManager.OnErrorEvent += AddressablesUtility.OnHandleError;

            // オーディオマネージャ生成
            await CreateAudioManager();

            // Fontタグが使用できるようにする
            AddDefaultFont();

            // Firebase初期化（先走りすぎるとiOSでトークンの初期化が終わっていない場合があるので少し遅延させる）
            int firebaseDelayTime = GetFirebaseDelayTime();
            UniTask firebaseInitTask = FirebaseInitializeAsync(firebaseDelayTime);

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            // SRDebugger初期化
            CruFramework.DebugMenu.Initialize();
#endif

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && PJFB_DEV
            APILogger.DeleteLog();
            CruFramework.DebugMenu.AddOption("API", $"Copy APILog", () => GUIUtility.systemCopyBuffer = APILogger.ReadLog());
            CruFramework.DebugMenu.AddOption("API", $"Copy FullAPILog", () => GUIUtility.systemCopyBuffer = APILogger.ReadFullLog());
#endif
            //コンフィグ設定適用
            Menu.AppConfig.ApplyConfig();
            
            CruFramework.Logger.Log("AppInitializer Initialize Complete");
            
            // Firebase初期化完了待ち
            await firebaseInitTask;
            
            isInitialized = true;
        }
        
        /// <summary>オーディオマネージャ生成</summary>
        public async static UniTask CreateAudioManager()
        {
            AsyncOperationHandle<GameObject> handle = AddressablesManager.LoadAssetAsync<GameObject>("Prefabs/AudioManager.prefab");
            await AddressablesManager.WaitHandle(handle);
            GameObject.Instantiate(handle.Result);
        }

        private static void AddDefaultFont()
        {
            TMP_FontAsset tempFont;
            // Check if we already have a reference to this font asset.
            MaterialReferenceManager.TryGetFontAsset(TMP_Settings.defaultFontAsset.hashCode, out tempFont);
            if (tempFont == null)
            {
                // Add new reference to the font asset as well as default material to the MaterialReferenceManager
                MaterialReferenceManager.AddFontAsset(TMP_Settings.defaultFontAsset);
            }
        }

        private static async UniTask FirebaseInitializeAsync(int delayMilliseconds = 0)
        {
            // 初期化遅延
            await UniTask.Delay(delayMilliseconds);
      
            // FirebaseとCrashlyticsを初期化
            await CruCrashLog.InitializeCrashLog(new [] { CruCrashLog.ServiceEnum.Crashlytics },
                backtraceUrl: string.Empty,
                onInitializedCB:(service, instance) =>
                {
                    switch (service)
                    {
                        case CruCrashLog.ServiceEnum.Crashlytics:
                        {
                            // DefaultInstanceの取得=インスタンスの生成になっているためこれやらなきゃクラッシュログが飛ばない…らしい（TODO: 中でやってて不要説アリ
                            var firebaseInstance = instance as Firebase.FirebaseApp;
                        }
                            break;
                    }
                    
                }, (CruCrashLog.EnvironmentEnum)AppEnvironment.CurrentEnvironment);

            FCMPushNotificationUtility.PreInitializeNotification();
            
            Logger.Log("Firebase Initialize Complete");
        }
        
        /// <summary>Firebase初期化を待機させる時間取得</summary>
        private static int GetFirebaseDelayTime()
        {
#if UNITY_IOS
            // iOSはデフォルトで100ms待機
            int delayDefaultTime = 100;
#else
            int delayDefaultTime = 0;
#endif
            
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            // デバコマが有効な場合は時間を上書き
            delayDefaultTime = PlayerPrefs.GetInt("firebaseInit", delayDefaultTime);
#endif
            return delayDefaultTime;
        }
    }
}
