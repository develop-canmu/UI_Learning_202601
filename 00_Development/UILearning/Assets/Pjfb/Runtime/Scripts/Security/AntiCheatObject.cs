using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.Common;
using Firebase.Analytics;

namespace Pjfb
{
    /// <summary>ページに必要なリソースの読み込みのUtilityクラス</summary>
    public class AntiCheatObject : MonoBehaviour
    {
        private enum CheatType
        {
            PlayerPrefs_Data,
            PlayerPrefs_Device
        }

        public static bool IsActive { get; set; } = false;

        private Queue<Action> pendingSendAnalytics { get; set; } = new Queue<Action>();

        #region MonoBehaviour Event

        private void Start()
        {
            // AntiCheat_チート検出イベント登録
            ObscuredPrefs.NotGenuineDataDetected += OnDataCheat; // PlayerPrefsの改竄検知
            ObscuredPrefs.DataFromAnotherDeviceDetected += OnLockCheat; // 他端末(DeviceID)のPlayerPrefsを検知（DeviceID誤検知の可能性が少しある）

            UnityApiResultsHolder.InitForAsyncUsage(true);
        }

        private void Update()
        {
            CheckAndSendAnalytics();
        }

        #endregion

        #region AntiCheat Event

        private void OnDataCheat()
        {
            CruFramework.Logger.LogWarning("AntiCheatObject.OnDataCheat");
            AddAnalytics(CheatType.PlayerPrefs_Data);
        }

        private void OnLockCheat()
        {
            CruFramework.Logger.LogWarning("AntiCheatObject.OnLockCheat");
#if UNITY_ANDROID
            // Only Android iOSは誤検知があるため
            AddAnalytics(CheatType.PlayerPrefs_Device);
#endif
        }

        #endregion

        [System.Diagnostics.Conditional("PJFB_REL")]
        private void CheckAndSendAnalytics()
        {
            if (!AppInitializer.IsInitialized || !IsActive) return;

            if (pendingSendAnalytics.TryDequeue(out var ac))
            {
                ac?.Invoke();
            }
        }

        private void AddAnalytics(CheatType type)
        {
            pendingSendAnalytics.Enqueue(() =>
            {
                FirebaseAnalytics.LogEvent(
                    FirebaseAnalytics.EventSelectContent,
                    new Parameter(FirebaseAnalytics.ParameterContentType, type.ToString()),
                    new Parameter(FirebaseAnalytics.ParameterItemName, UserData.UserDataManager.Instance.user.uMasterId)
                );
            });
        }
    }
}
