using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine;
using Logger = CruFramework.Logger;

#if !UNITY_EDITOR && UNITY_ANDROID
using Unity.Notifications.Android;
#elif !UNITY_EDITOR && UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Pjfb.Utility
{
    /// <summary>
    /// プッシュ通知のユーザー登録を行う
    /// </summary>
    public static class FCMPushNotificationUtility
    {
        private const int RetryFirebaseMessagingInitMaxCount = 5;
        private const int RetryFirebaseMessagingDelayMillisecond = 500;
        
        // FCMの初期化フラグ
        private static bool isInitializedFcm = false;

        /// <summary>
        /// プッシュ通知のユーザー登録
        /// </summary>
        public static async UniTask InitializePushStateAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // ユーザー作成後、初回の初期化であればプッシュ権限要求を行う
            if (LocalSaveManager.saveData.promptedPushPermission == false)
            {
                await RequestAuthorizationAsync(token);
                LocalSaveManager.saveData.promptedPushPermission = true;
                LocalSaveManager.Instance.SaveData();
            }
            
            // FCMの初期化が走るのでオンにする
            isInitializedFcm = true;
            
#if !UNITY_EDITOR
            // FCMトークンを送信（トークンは変わる可能性があるので毎回叩く）
            SendManualFcmTokenAsync(token);
#endif
            
            // 送信後にトークンが変わった時のcallbackの登録
            RegisterFirebaseCallback();
        }

        /// <summary>
        /// Firebaseのプッシュ通知callbackの登録
        /// </summary>
        private static void RegisterFirebaseCallback()
        {
#if !UNITY_EDITOR
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
#endif
        }
        
        /// <summary>
        /// トークンを手動で送信する
        /// </summary>
        private static void SendManualFcmTokenAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    string token = task.Result;
                    TokenStringReceived(token);
                }
                else if (task.IsFaulted)
                {
                    // Token取得に失敗する場合、再チャレンジする
                    TryFetchFcmTokenWithRetryAsync(cancellationToken).Forget();
                }
            });
        }

        /// <summary>
        /// APNsトークン準備を考慮してFCMトークン取得をリトライします。
        /// </summary>
        private static async UniTask TryFetchFcmTokenWithRetryAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var delayMs = RetryFirebaseMessagingDelayMillisecond;
            for (var attempt = 0; attempt < RetryFirebaseMessagingInitMaxCount; attempt++)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    var fetched = await FirebaseMessaging.GetTokenAsync();
                    TokenStringReceived(fetched);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"FCM GetToken retry {attempt + 1} failed: {ex.Message}");
                }

                await UniTask.Delay(delayMs, cancellationToken: token);
            }

            Logger.LogWarning("FCM token fetch failed after retries. APNs token may be unavailable yet.");
        }

        /// <summary>
        /// アカウント削除後メッセージのプッシュ通知を解除する
        /// </summary>
        public static void UnregisterFirebaseMessageCallback()
        {
            // FCMを初期化していない場合はスキップ（引き継ぎ時にインスタンス初期化が走ってOneSignalとの共存に不都合）
            if (isInitializedFcm == false) return;
            
#if !UNITY_EDITOR
            FirebaseMessaging.DeleteTokenAsync();
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
#endif
            Logger.Log("FCM 購読解除");
        }

        /// <summary>
        /// Crz7サーバーにFCMトークンを登録します
        /// </summary>
        private static async UniTask RegisterPushUserIdAsync(string pushUserId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(pushUserId))
            {
                return;
            }

            Logger.Log($"RegisterPushUserIdAsync: {pushUserId}");
            var userRegisterPushUserIdAPIRequest = new UserRegisterPushUserIdAPIRequest();
            var userRegister = new UserRegisterPushUserIdAPIPost();
            userRegister.pushUserId = pushUserId;
            userRegister.pushType = (long)UserRegisterPushUserIdAPIRequest.PushType.Fcm;
            userRegisterPushUserIdAPIRequest.SetPostData(userRegister);
            await APIManager.Instance.Connect(userRegisterPushUserIdAPIRequest);
        }

        /// <summary>
        /// FCMトークン発行コールバック
        /// </summary>
        private static void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Logger.Log("Received Registration Token: " + token.Token);
            RegisterPushUserIdAsync(token.Token).Forget();
        }

        /// <summary>
        /// FCMトークン発行コールバック
        /// </summary>
        private static void TokenStringReceived(string token)
        {
            Logger.Log("Received Registration Token: " + token);
            RegisterPushUserIdAsync(token).Forget();
        }

        /// <summary>
        /// FCM通知受信コールバック
        /// </summary>
        private static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Logger.Log("Push OnMessageReceived");

            // メッセージに通知ペイロードが含まれていることを確認
            if (e.Message.Notification != null)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                try
                {
                    // Firebaseから受け取ったタイトルと本文
                    var title = e.Message.Notification.Title;
                    var body = e.Message.Notification.Body;

                    // Androidローカル通知を作成
                    var notification = new AndroidNotification()
                    {
                        Title = title,
                        Text = body,
                        FireTime = DateTime.Now,
                        SmallIcon = LocalPushNotificationUtility.NotificationSmallIconName,
                        LargeIcon = LocalPushNotificationUtility.NotificationLargeIconName,
                    };

                    // 通知を送信して表示
                    AndroidNotificationCenter.SendNotification(notification, LocalPushNotificationUtility.AndroidChannelId);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Android local notification failed: {ex}");
                }
#endif
            }
        }

        /// <summary>
        /// FirebaseMessagingの利用開始
        /// </summary>
        public static void PreInitializeNotification()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            // アプリがフォアグランドの場合は自分でローカルプッシュから通知を着火できるようにチャンネル設定
            var ch = new AndroidNotificationChannel() {
                Id = LocalPushNotificationUtility.AndroidChannelId,
                Name = LocalPushNotificationUtility.AndroidTitle,
                Description = LocalPushNotificationUtility.AndroidDescription,
                Importance = Importance.High
            };
            AndroidNotificationCenter.RegisterNotificationChannel(ch);
#endif
        }

        /// <summary>
        /// プッシュ通知利用権限の要求
        /// </summary>
        private static async UniTask RequestAuthorizationAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

#if !UNITY_EDITOR && UNITY_ANDROID
            try
            {
                var request = new PermissionRequest();
                while (request.Status == PermissionStatus.RequestPending)
                {
                    await UniTask.Yield(token);
                    Logger.Log("Request Android Notification Permission:" + request.Status);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Android notification permission request failed: {ex}");
            }
#elif !UNITY_EDITOR && UNITY_IOS
            try
            {
                var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
                using var req = new AuthorizationRequest(authorizationOption, true);
                while (!req.IsFinished)
                {
                    await UniTask.Yield(token);
                };

                var res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                Logger.Log(res);
            }
            catch (Exception ex)
            {
                Logger.LogError($"iOS notification permission request failed: {ex}");
            }

#elif UNITY_EDITOR
            // エディタの場合は何もしない
            await UniTask.CompletedTask;
#endif
        }
    }
}
