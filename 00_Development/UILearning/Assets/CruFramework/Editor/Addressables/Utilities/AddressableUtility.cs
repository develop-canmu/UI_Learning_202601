using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Addressables;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    public static class AddressableUtility
    {
        /// <summary>設定ファイルを取得する</summary>
        public static AddressableAssetSettings GetAddressableAssetSettings()
        {
            // 設定ファイルが存在しなければ作成
            if(!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
            }
            return AddressableAssetSettingsDefaultObject.Settings;
        }
        
        /// <summary>キャッシュ設定を取得する</summary>
        public static CacheInitializationSettings GetCacheInitializationSettings()
        {
            string path = $"{GetAddressableAssetSettings().ConfigFolder}/{nameof(CacheInitializationSettings)}.asset";
            CacheInitializationSettings asset = AssetDatabase.LoadAssetAtPath<CacheInitializationSettings>(path);
            
            // ファイルが存在しなければ作成
            if(asset == null)
            {
                asset = ScriptableObject.CreateInstance<CacheInitializationSettings>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }
            
            // 登録されてなければ追加
            AddressableAssetSettings settings = GetAddressableAssetSettings();
            if(!settings.InitializationObjects.Contains(asset))
            {
                settings.AddInitializationObject(asset);
                AssetDatabase.SaveAssets();
            }
            
            return asset;
        }
        
        /// <summary>エディタ設定を取得する</summary>
        public static AddressableCustomSettingsObject GetAddressableCustomSettings()
        {
            string path = $"{GetAddressableAssetSettings().ConfigFolder}/{nameof(AddressableCustomSettingsObject)}.asset";
            AddressableCustomSettingsObject asset = AssetDatabase.LoadAssetAtPath<AddressableCustomSettingsObject>(path);
            // ファイルが存在しなければ作成
            if(asset == null)
            {
                asset = ScriptableObject.CreateInstance<AddressableCustomSettingsObject>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }
            return asset;
        }
        
        public static void SetDirty(UnityEngine.Object obj, Action callback, string context = "")
        {
            Undo.RegisterCompleteObjectUndo(obj, context);
            callback.Invoke();
            EditorUtility.SetDirty(obj);
        }

        /// <summary> PlayModeからAddressableを利用するかをセット </summary>
        [InitializeOnLoadMethod]
        private static void SetUseAddressable()
        {
            // 現在のプレイモードをランタイムに反映
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            bool isUse = settings.ActivePlayModeDataBuilder.GetType() == typeof(BuildScriptPackedPlayMode);
            AddressablesManager.SetUseAddressable(isUse);
        }
    }
}
