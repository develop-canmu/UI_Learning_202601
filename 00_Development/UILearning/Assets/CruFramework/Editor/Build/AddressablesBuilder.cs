#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using CruFramework.Editor.Addressables;
using Pjfb.Editor.Spine;

namespace CruFramework.Editor.Build
{
    public class AddressablesBuilder : BuilderBase
    {
        [MenuItem(EditorMenu.BuildMenuPath + "/Addressables/Android/Remote")]
        public static void BuildRemoteAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            Build("Editor", "1");
        }

        [MenuItem(EditorMenu.BuildMenuPath + "/Addressables/Android/Local")]
        public static void BuildLocalAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            Build("Editor", "-1");
            if(Directory.Exists("ServerData"))
            {
                Directory.Delete("ServerData", true);
            }
        }

        [MenuItem(EditorMenu.BuildMenuPath + "/Addressables/iOS/Remote")]
        public static void BuildRemoteiOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            Build("Editor", "1");
        }

        [MenuItem(EditorMenu.BuildMenuPath + "/Addressables/iOS/Local")]
        public static void BuildLocaliOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            Build("Editor", "-1");
            if(Directory.Exists("ServerData"))
            {
                Directory.Delete("ServerData", true);
            }
        }

        private static readonly string AssetVersionName = "AssetVersion";
        
        /// <summary>コマンドラインからのビルド</summary>
        public static void CommandLineBuild()
        {
            string environment = GetCommandLineArg<string>("buildType");
            string assetVersion = GetCommandLineArg<string>("assetVersion");
            
            string buildTargetStr = GetCommandLineArg<string>("buildTarget");
            BuildTarget buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTargetStr);
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, buildTarget);
            }

            Build( environment,  assetVersion);
        }

        /// <summary>リモートビルド</summary>
        public static AddressablesPlayerBuildResult Build(string environment, string assetVersion)
        {
            // アドレッサブル設定
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            // ビルド設定
            AddressablesBuildSettings buildSettings = GetAddressablesBuildSettings(environment);
            
            // アプリビルド時に勝手にビルドしないようにする
            settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;
            
            // プロファイル変更
            string profileId = ChangeProfile(buildSettings.ProfileName);
            // アセットバージョン設定
            settings.profileSettings.SetValue(profileId, AssetVersionName, assetVersion);
            // ローカルパス設定
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kLocalBuildPath, AddressableAssetSettings.kLocalBuildPathValue);
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kLocalLoadPath, AddressableAssetSettings.kLocalLoadPathValue);
            // リモートパス設定
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, buildSettings.RemoteBuildPath);
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, buildSettings.RemoteLoadPath);
            
            // リモートカタログをビルドする
            settings.BuildRemoteCatalog = true;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
            // リモートのカタログ名
            switch (buildSettings.CatalogNaming)
            {
                case AddressablesBuildSettings.CatalogNameType.Version:
                    settings.OverridePlayerVersion = assetVersion;
                    break;
                case AddressablesBuildSettings.CatalogNameType.String:
                    settings.OverridePlayerVersion = buildSettings.OverridePlayerVersion;
                    break;
            }

            // 1.12.1以降はこのオプションを有効にすると効率的なバンドルレイアウトを生成する
            settings.ContiguousBundles = true;
            // 初期化タイミングをコントロールする
            settings.DisableCatalogUpdateOnStartup = true;

            // アドレスを振る
            AddressablesUtility.SetAddress();
            // リモートアセットが参照してるリソースがアセットバンドルになっていない場合の依存関係解決
            AddressablesUtility.RegisterDependenciesInAssetBundles();
            
            // 全てのグループをビルドに含める
            foreach (AddressableAssetGroup group in settings.groups)
            {
                BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null) continue;
                schema.IncludeInBuild = group.entries.Count > 0;
            }

            // ビルダー指定
            foreach (IDataBuilder builder in settings.DataBuilders)
            {
                if (builder.GetType() == typeof(BuildScriptPackedMode))
                {
                    // インデックス切り替え
                    settings.ActivePlayerDataBuilderIndex = settings.DataBuilders.IndexOf((ScriptableObject)builder);
                }
            }
            
            // ローカルのアセットがリモートを参照していないかチェック
            AddressablesUtility.CheckForDependenciesOnRemote();

            // キャッシュ削除
            CacheClear();
            // ビルド
            AddressablesPlayerBuildResult buildResult;
            AddressableAssetSettings.BuildPlayerContent(out buildResult);

            return buildResult;
        }

        /// <summary>ビルドキャッシュ削除</summary>
        [MenuItem(EditorMenu.BuildMenuPath + "/Addressables/CacheClear")]
        private static void CacheClear()
        {
            AddressableAssetSettings.CleanPlayerContent();
            BuildCache.PurgeCache(false);
        }

        /// <summary>プロファイルを切り替える</summary>
        private static string ChangeProfile(string profileName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            // プロファイルがあるか調べる
            string profileId = settings.profileSettings.GetProfileId(profileName);
            // 無ければ追加する
            if (string.IsNullOrEmpty(profileId))
            {
                profileId = settings.profileSettings.AddProfile(profileName, settings.activeProfileId);
                settings.activeProfileId = profileId;
            }

            // パラメータ追加
            settings.profileSettings.CreateValue(AssetVersionName, "1");

            return profileId;
        }

        /// <summary>ビルド設定ファイル取得</summary>
        private static AddressablesBuildSettings GetAddressablesBuildSettings(string profileName)
        {
            // string profileName = GetCommandLineArg<string>("-profileName", "Editor");
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AddressablesBuildSettings)} {profileName}");
            if (guids.Length == 0)
            {
                throw new Exception($"ビルド設定が見つかりません。{profileName}が存在するか確認してください。");
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            Debug.Log($"Loaded BuildSettings : {assetPath}");
            return AssetDatabase.LoadAssetAtPath<AddressablesBuildSettings>(assetPath);
        }
    }
}

#endif
