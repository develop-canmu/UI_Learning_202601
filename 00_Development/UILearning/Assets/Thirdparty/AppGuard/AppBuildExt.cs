using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace ThirdParty.AppGuard.Editor
{
    public class AppBuildExt
    {
        private const string PrefsKeyAppVersion = "AppBuildExt.BuildParam.AppVersion";
        private const string PrefsKeyVersionCode = "AppBuildExt.BuildParam.VersionCode";
        private const string PrefsKeyKeyStoreDir = "AppBuildExt.BuildParam.KeyStoreDir";

        public string AppVersion { get; private set; }
        public string VersionCode { get; private set; }
        public string KeystoreDir { get; private set; }

        public static void GetBuildParam(BuildTarget buildTarget, string env, Action<AppBuildExt> cb)
        {
            var result = new AppBuildExt();
            result.AppVersion = PlayerPrefs.GetString(PrefsKeyAppVersion, null);
            result.VersionCode = PlayerPrefs.GetString(PrefsKeyVersionCode, null);
            result.KeystoreDir = PlayerPrefs.GetString(PrefsKeyKeyStoreDir, null);

            var titleAppVersion = "App Version";
            var titleVersionCode = "Version Code";
            var titleKeystoreDir = "KeyStore Directory";
            var elements = new Dictionary<string, string>(){
                {titleAppVersion, result.AppVersion },
                {titleVersionCode, result.VersionCode },
            };
            if (buildTarget == BuildTarget.Android)
            {
                elements.TryAdd(titleKeystoreDir, result.KeystoreDir);
            }
            // Show Window
            InputsWindow.Open($"Build Param [{buildTarget}_{env}]", elements, (inputs) =>
            {
                if (inputs == null)
                {
                    Debug.Log("=== Build Cancel ===");
                    cb = null;
                    return;
                }

                if (inputs.TryGetValue(titleAppVersion, out var updateAppVersion))
                {
                    result.AppVersion = updateAppVersion;
                    PlayerPrefs.SetString(PrefsKeyAppVersion, result.AppVersion);
                }
                if (inputs.TryGetValue(titleVersionCode, out var updateVersionCode))
                {
                    result.VersionCode = updateVersionCode;
                    PlayerPrefs.SetString(PrefsKeyVersionCode, result.VersionCode);
                }
                if (inputs.TryGetValue(titleKeystoreDir, out var updateKeystoreDirPath))
                {
                    result.KeystoreDir = updateKeystoreDirPath;
                    PlayerPrefs.SetString(PrefsKeyKeyStoreDir, result.KeystoreDir);
                }
                PlayerPrefs.Save();

                cb?.Invoke(result);
                cb = null;
            });
        }
    }
}