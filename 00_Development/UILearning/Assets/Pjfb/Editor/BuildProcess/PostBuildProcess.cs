using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif


namespace Pjfb.Editor.PostBuildProcess
{
    public class PostBuildProcess
    {
        // OneSignalのPostProcessが45番で実行していた名残
        private const int PjfbPostProcessBuildCallbackOrder = 46;
        
        [PostProcessBuild(PjfbPostProcessBuildCallbackOrder)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            Debug.Log("BuildTest: OnPostProcess");
            switch (buildTarget)
            {
                case BuildTarget.iOS:
                    IOSPostBuildProcess(path);
                    break;
                case BuildTarget.Android:
                    break;
            }
        }
        
        private static void IOSPostBuildProcess(string path)
        {
#if UNITY_IOS
            var pjPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var pj = new PBXProject();
            pj.ReadFromString(File.ReadAllText(pjPath));
            var targetGuid = pj.GetUnityMainTargetGuid();
            
            // Notification利用のためにframeworkを追加
            pj.AddFrameworkToProject(targetGuid, "UserNotifications.framework", false);
            
            // Adjustで求められているフレームワークの追加.
            pj.AddFrameworkToProject(targetGuid, "iAd.framework", false);
            pj.AddFrameworkToProject(targetGuid, "AdSupport.framework", false);

            pj.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", false);
            pj.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
            
            pj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            
            targetGuid = pj.GetUnityFrameworkTargetGuid();
            pj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // AddGuard で必要なFramework（AdSupport.framework 上記でやっているので省略）
            pj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
            
            // Info.pList 項目追加
            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDict = plist.root;
            // 言語日本語設定
            rootDict.SetString("CFBundleDevelopmentRegion", "ja");
            var localizeArray = rootDict.CreateArray("CFBundleLocalizations");
            localizeArray.AddString("ja");
            
            // UIBackgroundModesにremote-notificationを追加
            rootDict.CreateArray("UIBackgroundModes");
            rootDict["UIBackgroundModes"].AsArray().AddString("remote-notification");
            
            // 本番か開発かで指定するパラメータ
            string apsEnvironmentName = "aps-environment";
            string envType = "development";
            // 本番ビルドのバンドルIDかどうか
            if (PlayerSettings.applicationIdentifier == "jp.pjfb")
            {
                envType = "production";
            }
            
            // Entitlementsファイルの作成
            string entitlementsFileName = "Unity-iPhone.entitlements";
            string entitlementsFilePath = Path.Combine(path, entitlementsFileName);
            PlistDocument entitlementsPlist = new PlistDocument();
            entitlementsPlist.root.SetString(apsEnvironmentName, envType);
            entitlementsPlist.WriteToFile(entitlementsFilePath);

            // EntitlementsファイルのパスをXcodeプロジェクトに設定
            pj.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsFileName);
            // plistにaps-environmentを追加
            rootDict.SetString(apsEnvironmentName, envType);
            
            
            // 使わないけど設定しておかないと警告が出るため
            rootDict.SetString("NSLocationAlwaysUsageDescription","we do not utilize your location information outside or inside of this app");
            rootDict.SetString("NSLocationWhenInUseUsageDescription","we do not utilize your location information outside or inside of this app");
            
            // プロパティリスト書き込み
            File.WriteAllText(plistPath,plist.WriteToString());
            // PBX書き込み
            File.WriteAllText(pjPath, pj.WriteToString());
            
            // MetalDisplayLinkを使っているとフリーズする問題があるため無効化する
            // UnityAppController.hの書き換え
            string unityAppControllerPath = Path.Combine(path, "Classes/UnityAppController.h");
            if (File.Exists(unityAppControllerPath) == false)
            {
                // ないときはログだけ出す（ビルドはコケさせる。2バイト文字怪しいので英語っぽく）
                CruFramework.Logger.LogError($"{unityAppControllerPath} is not found. Metal Display Link patch is not applied.");
            }
            
            string text = File.ReadAllText(unityAppControllerPath);
            string replaceText = text.Replace(
                "#define UNITY_USES_METAL_DISPLAY_LINK (UNITY_HAS_IOSSDK_17_0 || UNITY_HAS_TVOSSDK_17_0)",
                "#define UNITY_USES_METAL_DISPLAY_LINK 0");
            // 置き換えに失敗したら例外を投げる
            if (text == replaceText)
            {
                throw new InvalidOperationException("Failed to replace UNITY_USES_METAL_DISPLAY_LINK in UnityAppController.h. The target string was not found.");
            }
            File.WriteAllText(unityAppControllerPath, replaceText);
            
#endif
        }

        private static void AndroidPostBuildProcess(string path)
        {
        }
    }
}