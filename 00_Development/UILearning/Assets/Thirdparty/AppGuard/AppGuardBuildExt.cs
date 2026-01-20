using System;
using System.IO;
using DiresuUnity.Editor;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace ThirdParty.AppGuard.Editor
{
    public class AppGuardBuildExt
    {
        private static ObjectLoader<AppGuardBuildSetting> AppGuardSetting { get; } = new ObjectLoader<AppGuardBuildSetting>();

        /// <summary>
        /// Set AddGuard Setting (Only Android)
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="outputPath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static BuildPlayerOptions SetSettings(string environment, string outputPath, BuildPlayerOptions options)
        {
            var dirPath = Path.GetDirectoryName(outputPath);
            var fileName = Path.GetFileNameWithoutExtension(outputPath);
            var ext = Path.GetExtension(outputPath);
            Debug.Log($"Parse Path: {dirPath} | {fileName} | {ext}");
            var setting = AppGuardSetting.GetObject(environment);
            Debug.Log($"Suffix: {setting.tmpApkFileSuffix}");
            options.locationPathName = Path.Combine(dirPath, $"{fileName}{setting.tmpApkFileSuffix}{ext}");
            Debug.Log($"Override locationPathName: {options.locationPathName}");

            return options;
        }

        /// <summary>
		/// Protect AppGuard
		/// </summary>
		/// <param name="env"></param>
		/// <param name="unprotectedApk"></param>
		/// <param name="originApkPath"></param>
		/// <param name="setting"></param>
        public static void ProtectAppGuard(BuildTarget target, string env, string locationPath, string originPath)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    ProtectAndroid(env, locationPath, originPath);
                    break;
                case BuildTarget.iOS:
                    ProtectIOS(env, locationPath);
                    break;
                default:
                    Debug.LogError(LogFormat($"Build Error: BuildTarget not support >>> [{target}]"));
                    return;
            }

            Debug.Log(LogFormat("<color=green>Complete Upload/Download Process</color>"));
        }


        private static void ProtectAndroid(string env, string locationApkPath, string originApkPath)
        {
            Debug.Log(LogFormat("Android Start Upload/Download Process"));
            var setting = AppGuardSetting.GetObject(env);
            var keystoreName = PlayerSettings.Android.keystoreName;
            var keystorePassword = PlayerSettings.Android.keystorePass;
            var keystoreAlias = PlayerSettings.Android.keyaliasName;
            var keystoreAliasPassword = PlayerSettings.Android.keyaliasPass;
            var opt = new AndroidProtectionOptions
            {
                AppKey = setting.appKey,
                Version = setting.protectionVersionAndroid,
                Plan = setting.plan,
                TargetFilePath = locationApkPath,
                OutputFilePath = originApkPath,
                KeystoreName = keystoreName,
                KeystorePass = keystorePassword,
                KeyaliasName = keystoreAlias,
                KeyaliasPass = keystoreAliasPassword,
                CertificateFingerprints = setting.androidSha256List
            };
            Debug.Log(LogFormat($"Android Options\n{opt}"));
            try
            {
                AppGuardAndroidProtector.Protect(opt);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private static void ProtectIOS(string env, string locationPath)
        {
            Debug.Log(LogFormat("iOS Start Upload/Download Process"));
            var setting = AppGuardSetting.GetObject(env);
            var opt = new IOSProtectionOptions
            {
                AppKey = setting.appKey,
                Version = setting.protectionVersionIOS,
                Plan = setting.plan
            };
            
            // シェルスクリプト実行時はオプション引数のキーを参照しないので直接設定を書き換える
            AppGuardSettings.IOS.AppKey = setting.appKey;
            
            AppGuardIOSShellScriptGenerator.GenerateShellScript(locationPath, opt);
        }

        private static string LogFormat(string msg) => $"[AppGuard] {msg}";
    }
}