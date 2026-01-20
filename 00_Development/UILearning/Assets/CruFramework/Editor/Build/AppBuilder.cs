using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using ThirdParty.AppGuard.Editor;
using Unity.Android.Types;
using UnityEditor.Android;
using AndroidBuildSystem = UnityEditor.AndroidBuildSystem;

namespace CruFramework.Editor.Build
{
    public class AppBuilder : BuilderBase
    {
        static readonly string androidOutputPath = $"Build/{BuildTarget.Android}/app.apk";
        static readonly string iOSOutputPath = $"Build/{BuildTarget.iOS}";


        private static void BuildAndroid(string env)
        {
            AppBuildExt.GetBuildParam(BuildTarget.Android, env, (res) =>
            {
                BuildApp(BuildTarget.Android, env, androidOutputPath, res.AppVersion, res.VersionCode, res.KeystoreDir);
            });
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/Android/Development")]
        private static void BuildAndroidDevelopment()
        {
            BuildAndroid("Development");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/Android/Staging")]
        private static void BuildAndroidStaging()
        {
            BuildAndroid("Staging");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/Android/Release")]
        private static void BuildAndroidRelease()
        {
            BuildAndroid("Release");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/Android/Editor")]
        private static void BuildAndroidEditor()
        {
            BuildAndroid("Editor");
        }

        private static void BuildiOS(string env)
        {
            AppBuildExt.GetBuildParam(BuildTarget.iOS, env, (res) =>
            {
                BuildApp(BuildTarget.iOS, env, iOSOutputPath, res.AppVersion, res.VersionCode);
            });
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/iOS/Development")]
        private static void BuildiOSDevelopment()
        {
            BuildiOS("Development");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/iOS/Staging")]
        private static void BuildiOSStaging()
        {
            BuildiOS("Staging");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/iOS/Release")]
        private static void BuildiOSRelease()
        {
            BuildiOS("Release");
        }
        [MenuItem(EditorMenu.BuildMenuPath + "/App/iOS/Editor")]
        private static void BuildiOSEditor()
        {
            BuildiOS("Editor");
        }


        /// <summary>ビルドに必ず含むシーン</summary>
        private static readonly string[] defaultBuildScenes = new string[]
        {
        };

        /// <summary>ビルド必ず含むシンボル</summary>
        private static readonly string[] defaultDefineSymbols = new string[]
        {
        };

        /// <summary>コマンドラインからのビルド</summary>
        public static void CommandLineBuild()
        {
            var buildTargetStr = GetCommandLineArg<string>("buildTarget");
            var buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTargetStr);
            var environment = GetCommandLineArg<string>("buildType");
            var outputPath = GetCommandLineArg<string>("outputPath");
            var appVersion = GetCommandLineArg<string>("appVersion");
            var versionCode = GetCommandLineArg<string>("versionCode");
            var keystorePath = GetCommandLineArg<string>("keystorePath", "");
            var developmentBuild = GetCommandLineArg<bool>("developmentBuild");
            var commitHash = GetCommandLineArg<string>("commitHash", "develop");

            BuildApp(buildTarget, environment, outputPath, appVersion, versionCode, keystorePath, developmentBuild, commitHash: commitHash);
        }

        /// <summary>アプリビルド</summary>
        public static BuildReport BuildApp(BuildTarget buildTarget, string environment, string outputPath, string appVersion = null, string versionCode = "", string keystorePath = "", bool developmentBuild = false, string commitHash = "develop")
        {
            // ビルドグループ取得
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            // プラットフォーム切り替え
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, buildTarget);

            // 設定ファイル検索
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AppBuildSettings)} {environment}");
            // 設定ファイルが存在しない
            if (guids.Length == 0)
            {
                throw new Exception($"ビルド設定が見つかりません。{environment}が存在するか確認してください。");
            }

            // 設定ファイル取得
            string buildSettingPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            AppBuildSettings buildSettings = AssetDatabase.LoadAssetAtPath<AppBuildSettings>(buildSettingPath);
            Debug.Log($"Loaded BuildSettings : {buildSettingPath}");

            // 製品名
            PlayerSettings.productName = buildSettings.ProductName;
            // バンドルID
            PlayerSettings.applicationIdentifier = buildSettings.BundleId;
            PlayerSettings.SetApplicationIdentifier(buildGroup, buildSettings.BundleId);
            // バージョン
            PlayerSettings.bundleVersion = appVersion;
            
            #if BACKTRACE_INSTALLED
            var param =  Engine.CrashLog.CruCrashLog.GetParameterData();
            param.commitHash = commitHash;
            Engine.CrashLog.CruCrashLog.OverwriteParameters(param);
            #endif

            // 各プラットフォーム毎の設定
            if (buildTarget == BuildTarget.iOS)
            {
                //Re-import GoogleService-Info.plist
                guids = AssetDatabase.FindAssets($"{buildSettings.BundleId}_GoogleService-Info");
                if (guids.Length != 0)
                {
                    foreach (var guid in guids)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(guid);
                        if (p.EndsWith(".plist"))
                        {
                            var targetPlist = AssetDatabase.RenameAsset(p, "GoogleService-Info.plist");
                            AssetDatabase.ImportAsset(targetPlist, ImportAssetOptions.ForceUpdate);
                            break;
                        }
                    }
                }

                // プロビジョニングプロファイルID
                PlayerSettings.iOS.iOSManualProvisioningProfileID = buildSettings.ProvisioningProfileId;
                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                // チームID
                PlayerSettings.iOS.appleDeveloperTeamID = buildSettings.SigningTeamId;
                PlayerSettings.iOS.appleEnableAutomaticSigning = false;

                // ビルドナンバー
                PlayerSettings.iOS.buildNumber = versionCode;
            }
            else if (buildTarget == BuildTarget.Android)
            {
                //Re-import google-services.json
                guids = AssetDatabase.FindAssets("google-services");
                if (guids.Length != 0)
                {
                    foreach (var guid in guids)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(guid);
                        if (p.EndsWith(".json"))
                        {
                            AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
                            break;
                        }
                    }
                }

                // キーストア
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = Path.Combine(keystorePath, buildSettings.KeystoreName);
                PlayerSettings.Android.keystorePass = buildSettings.KeystorePass;
                PlayerSettings.Android.keyaliasName = buildSettings.KeyAliasName;
                PlayerSettings.Android.keyaliasPass = buildSettings.KeyAliasPass;

                // バージョンコード
                PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);

                // ビルドシステム
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                EditorUserBuildSettings.buildAppBundle = environment == "release";
                UserBuildSettings.DebugSymbols.format = DebugSymbolFormat.Zip;
                UserBuildSettings.DebugSymbols.level = environment == "release" ? DebugSymbolLevel.Full : DebugSymbolLevel.None;
            }

            // シンボル定義
            List<string> defineList = new List<string>(defaultDefineSymbols);
            foreach (string define in buildSettings.DefineSymbols)
            {
                if (defineList.Contains(define)) continue;
                defineList.Add(define);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, string.Join(";", defineList));

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            // シーン
            List<string> sceneList = new List<string>(defaultBuildScenes);
            foreach (string scene in buildSettings.Scenes)
            {
                if (sceneList.Contains(scene)) continue;
                sceneList.Add(scene);
            }
            buildPlayerOptions.scenes = sceneList.ToArray();
            // ビルド場所
            buildPlayerOptions.locationPathName = outputPath;
            // プラットフォーム設定
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.targetGroup = buildGroup;
            // 開発ビルド
            if (buildSettings.DevelopmentBuild || developmentBuild)
            {
                buildPlayerOptions.options = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
            }

            // 除外アセットを削除する
            foreach (var asset in buildSettings.ExcludedAsset)
            {
                var path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }

            }

#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

            // ローカルアセットビルド
            AddressablesBuilder.Build(environment, "-1");

#endif
            // AppGuard Set Setting
            var options = (buildTarget == BuildTarget.Android) ? AppGuardBuildExt.SetSettings(environment, outputPath, buildPlayerOptions) : buildPlayerOptions;

            // アプリビルド
            var resultMsg = "BUILD SUCCESSFUL";
            var report = BuildPipeline.BuildPlayer(options);
            if (report != null && report.summary.result != BuildResult.Succeeded)
            {
                Debug.Log("BUILD FAILED:");
                Debug.Log(report);
                Debug.Log("TOTAL ERRORS: " + report.summary.totalErrors);
                var error = "";
                foreach (var stepInfo in report.steps)
                {
                    foreach (var msg in stepInfo.messages)
                    {
                        if (msg.type != LogType.Log && msg.type != LogType.Warning)
                        {
                            error += msg.content + "\n";
                        }
                    }
                }
                Debug.Log(error);
                resultMsg = "BUILD FAILED END";
            }
            Debug.Log(resultMsg);

            // AppGuard Run Protect
            AppGuardBuildExt.ProtectAppGuard(buildTarget, environment, options.locationPathName, outputPath);

            return report;
        }
    }
}
