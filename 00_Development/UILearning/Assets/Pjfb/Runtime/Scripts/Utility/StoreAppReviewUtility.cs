using System;
using System.Threading;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Pjfb
{
    public class StoreAppReviewUtility
    {
        public enum StoreAppReviewErrorCodeEnum
        {
            NoError,
            OpenWindowError,
            InvalidStoreError
        }

        public static void OpenReviewWindow(Action<StoreAppReviewErrorCodeEnum> CompleteCallback, CancellationToken token = default)
        {
#if UNITY_IOS
            UnityEngine.iOS.Device.RequestStoreReview();
            CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.NoError);
#elif UNITY_ANDROID
            LaunchReviewWindowAndroid(token, CompleteCallback: ret =>
            {
                CompleteCallback.Invoke(ret);
                // Debug.Log($"[OpenReviewWindow] Completed");
            });
#else
            CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.NoError);
#endif
        }

#if UNITY_ANDROID
        private static StoreAppReviewErrorCodeEnum HandleAndroidError(ReviewErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ReviewErrorCode.NoError:
                    return StoreAppReviewErrorCodeEnum.NoError;
                case ReviewErrorCode.ErrorRequestingFlow:
                case ReviewErrorCode.ErrorLaunchingFlow:
                    return StoreAppReviewErrorCodeEnum.OpenWindowError;
                case ReviewErrorCode.PlayStoreNotFound:
                    return StoreAppReviewErrorCodeEnum.InvalidStoreError;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }

        // Reference: https://developer.android.com/guide/playcore/in-app-review/unity
        private static async void LaunchReviewWindowAndroid(CancellationToken cancellationToken,
            Action<StoreAppReviewErrorCodeEnum> CompleteCallback)
        {
            try
            {
                ReviewManager reviewManager = new ReviewManager();

                var requestReviewFlow = reviewManager.RequestReviewFlow();
                while (requestReviewFlow is { IsDone: false })
                {
                    await Task.Delay(10, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.OpenWindowError);
                        return;
                    };
                }

                if (requestReviewFlow.Error != ReviewErrorCode.NoError)
                {
                    //Debug.Log($"[SDK] Request {requestReviewFlow.Error.ToString()}");
                    CompleteCallback.Invoke(HandleAndroidError(requestReviewFlow.Error));
                    return;
                }

                var playReviewInfo = requestReviewFlow.GetResult();
                // Debug.Log($"[SDK] Request OK {playReviewInfo}");

                var launchReviewFlow = reviewManager.LaunchReviewFlow(playReviewInfo);
                while (launchReviewFlow is { IsDone: false })
                {
                    await Task.Delay(10, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.OpenWindowError);
                        return;
                    };
                }

                if (launchReviewFlow.Error != ReviewErrorCode.NoError)
                {
                    //Debug.Log($"[SDK] Launch {requestReviewFlow.Error.ToString()}");
                    CompleteCallback.Invoke(HandleAndroidError(launchReviewFlow.Error));
                    return;
                }

                //Debug.Log($"[SDK] Launch OK {launchReviewFlow.IsSuccessful}");
                // The flow has finished. The API does not indicate whether the user
                // reviewed or not, or even whether the review dialog was shown. Thus, no
                // matter the result, we continue our app flow.
                CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.NoError);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                CompleteCallback.Invoke(StoreAppReviewErrorCodeEnum.OpenWindowError);
            }
        }
#endif
    }
}