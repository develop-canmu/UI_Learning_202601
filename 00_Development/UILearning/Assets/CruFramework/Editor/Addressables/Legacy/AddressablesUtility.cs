#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CruFramework.Editor.Addressable;
using CruFramework.Editor.Build;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace CruFramework.Editor.Addressables
{
    public static class AddressablesUtility
    {
        /// <summary>
        /// Unityアプデなどでアセットグループやエントリが更新されない場合があるため、全てのエントリと設定を一旦削除して再割り当てする
        /// </summary>
        [MenuItem("CruFramework/Addressables/Commands/ReserializeAllEntries")]
        public static void ReserializeAllEntries()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            
            // 全てのアセットを取得
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            // GUIDに変換
            string[] guids = assetPaths.Select(AssetDatabase.AssetPathToGUID).ToArray();
            // 全てのエントリを削除
            for (int index = 0; index < guids.Length; index++)
            {
                string guid = guids[index];
                settings.RemoveAssetEntry(guid, false);
                
                // プログレスバー
                float percent = (float)index / guids.Length;
                EditorUtility.DisplayProgressBar("Clearing All Entries", $"{percent * 100f :F1} % ({guid})", percent);
            }

            // 全てのグループ設定を削除
            settings.groups.Clear();
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            
            // 再割り当て
            SetAddress();
        }
        
        [MenuItem("CruFramework/Addressables/Commands/SetAddress")]
        public static void SetAddress()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableCustomSettingsObject customSettingsObject = AddressableUtility.GetAddressableCustomSettings();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            
            // プログレスバーを表示するか
            bool isShowProgressBar = !Application.isBatchMode;
            int index = 0;
            foreach (string assetPath in assetPaths)
            {
                AddressableImporter.AddOrMoveAssetEntry(assetPath, settings, customSettingsObject);
                index++;

                if (isShowProgressBar)
                {
                    // プログレスバー
                    float percent = (float)index / assetPaths.Length;
                    EditorUtility.DisplayProgressBar("Setting Address", $"{percent * 100f:F1} % ({assetPath})", percent);
                }
            }

            AssetDatabase.SaveAssets();

            if (isShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        /// <summary>
        /// 重複したアセットのキーを修正する。Dictionaryの追加エラーがでた時用
        /// </summary>
        [MenuItem("CruFramework/Addressables/Commands/FixDuplicateAddress")]
        public static void FixDuplicateAddress()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            // 重複管理用の辞書
            Dictionary<string, AddressableAssetEntry> entryDict = new Dictionary<string, AddressableAssetEntry>();
            
            foreach (AddressableAssetGroup assetGroup in settings.groups)
            {
                // m_SerializeEntries をリフレクションで取得(AssetGroup.entriesだと、すでに重複しているものはなくなっているため)
                FieldInfo rawEntries = assetGroup.GetType().GetField("m_SerializeEntries", BindingFlags.NonPublic | BindingFlags.Instance);
                if (rawEntries != null)
                {
                    // エントリリスト取得
                    List<AddressableAssetEntry> entryList = (List<AddressableAssetEntry>)rawEntries.GetValue(assetGroup);
                    // 処理前のサイズ
                    int beforeSize = entryList.Count;
                    
                    entryDict.Clear();
                    // 重複しているキーを取得
                    foreach (AddressableAssetEntry entry in entryList.ToList())
                    {
                        // 追加できる場合は続ける
                        if (entryDict.TryAdd(entry.guid, entry)) continue;
                        
                        // 重複している場合は削除
                        entryList.Remove(entry);
                        Debug.Log(@$"Removed ""{entry.guid}""   : [{assetGroup.Name}] {entry.AssetPath}");
                    }

                    // 処理後のサイズ
                    int afterSize = entryList.Count;
                    
                    // 処理前後で変化があれば、結果出力
                    if (beforeSize != afterSize)
                    {
                        rawEntries.SetValue(assetGroup, entryList);
                        Debug.Log($"Result [{assetGroup.Name}] : {afterSize - beforeSize}");
                    }
                }
            }
            
            // 再割り当て
            SetAddress();
        }
        
        /// <summary>アセットグループを取得</summary>
        public static AddressableAssetGroup GetOrCreateGroup(string groupName, bool setAsDefaultGroup = false, bool readOnly = true)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // グループが既に存在する場合はそれを返す
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group != null)
            {
                return group;
            }

            // 必要なスキーマ
            List<AddressableAssetGroupSchema> schemas = new List<AddressableAssetGroupSchema>()
            {
                ScriptableObject.CreateInstance<ContentUpdateGroupSchema>(),
                ScriptableObject.CreateInstance<BundledAssetGroupSchema>(),
            };

            // グループ生成
            group = settings.CreateGroup(groupName, setAsDefaultGroup, readOnly, true, schemas);

            return group;
        }
        
        /// <summary>デフォルト以外のすべてのグループを削除</summary>
        [MenuItem(EditorMenu.AddressablesMenuPath + "/Commands/RemoveAllEmptyEntry")]
        public static void RemoveAllEmptyEntry()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            foreach (AddressableAssetGroup group in settings.groups)
            {
                // 削除するエントリを集める
                List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    if(entry.MainAsset == null)
                    {
                        entries.Add(entry);
                    }
                }
                
                // 削除
                foreach (AddressableAssetEntry entry in entries)
                {
                    group.RemoveAssetEntry(entry);
                }
            }

        }

        /// <summary>デフォルト以外のすべてのグループを削除</summary>
        [MenuItem(EditorMenu.AddressablesMenuPath + "/Commands/RemoveAllGroupWithoutDefault")]
        public static void RemoveAllGroupWithoutDefault()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            for (int i = settings.groups.Count - 1; i >= 0; i--)
            {
                // デフォルトのグループは無視
                if (settings.groups[i].IsDefaultGroup()) continue;
                if (settings.groups[i].Name == AddressableAssetSettings.DefaultLocalGroupName) continue;
                // 削除
                settings.RemoveGroup(settings.groups[i]);
            }
        }

        /// <summary>デフォルト以外のすべての空グループを削除</summary>
        [MenuItem(EditorMenu.AddressablesMenuPath + "/Commands/RemoveAllEmptyGroupWithoutDefault")]
        public static void RemoveAllEmptyGroupWithoutDefault()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            for (int i = settings.groups.Count - 1; i >= 0; i--)
            {
                // デフォルトのグループは無視
                if (settings.groups[i].IsDefaultGroup()) continue;
                if (settings.groups[i].Name == AddressableAssetSettings.DefaultLocalGroupName) continue;
                // エントリが存在する場合は無視
                if (settings.groups[i].entries.Count > 0) continue;
                // 削除
                settings.RemoveGroup(settings.groups[i]);
            }
        }

        /// <summary>依存関係にあるアセットをアセットバンドルに登録</summary>
        [MenuItem(EditorMenu.AddressablesMenuPath + "/Commands/RegisterDependenciesInAssetBundles")]
        internal static void RegisterDependenciesInAssetBundles()
        {
            // アドレッサブル設定ファイル
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            // デフォルトグループのスキーマ設定
            BundledAssetGroupSchema schema = settings.DefaultGroup.GetSchema<BundledAssetGroupSchema>();
            schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);

            foreach (AddressableAssetGroup group in settings.groups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    // 依存関係のファイルパス取得
                    string[] dependencies = AssetDatabase.GetDependencies(entry.AssetPath);
                    foreach (string dependency in dependencies)
                    {
                        // 自身は無視
                        if (dependency == entry.AssetPath) continue;
                        // エディタフォルダのものは無視
                        if(dependency.Contains("Editor")) continue;
                        // ディレクトリは無視
                        if (AssetDatabase.GetMainAssetTypeAtPath(dependency) == typeof(DefaultAsset)) continue;
                        // スクリプトファイルは無視
                        if (AssetDatabase.GetMainAssetTypeAtPath(dependency) == typeof(MonoScript)) continue;
                        // guid取得
                        string guid = AssetDatabase.AssetPathToGUID(dependency);
                        // エントリが既に存在するか
                        AddressableAssetEntry dependencyEntry = settings.FindAssetEntry(guid);
                        // 存在しない場合はデフォルトのグループに登録
                        if (dependencyEntry == null)
                        {
                            settings.CreateOrMoveEntry(guid, settings.DefaultGroup, true);
                        }
                    }
                }
            }
        }
        
        /// <summary>ローカルのアセットバンドルがリモートに依存してないかチェックする</summary>
        [MenuItem(EditorMenu.AddressablesMenuPath + "/Analyze/CheckForDependenciesOnRemote")]
        public static void CheckForDependenciesOnRemote()
        {
            EditorUtility.DisplayProgressBar("ローカルファイルの依存関係をチェックしています", string.Empty, 0);
            
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            
            Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();
            // ローカルグループが依存しているアセットを取得する
            foreach (AddressableAssetGroup group in settings.groups)
            {
                // スキーマ取得
                BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                // スキーマが存在しない
                if(schema == null) continue;
                // ローカルグループ以外は無視
                if(schema.BuildPath.GetName(settings) != AddressableAssetSettings.kLocalBuildPath) continue;
                // 登録されてるアセットを調べる
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    // 依存関係にあるアセットを取得
                    dependencies.Add(entry.AssetPath, AssetDatabase.GetDependencies(entry.AssetPath));
                }
            }
            
            bool error = false;
            
            // ローカルグループが依存しているアセットがリモートグループに存在するか調べる
            foreach (KeyValuePair<string, string[]> dependency in dependencies)
            {
                int count = 0;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{dependency.Key}はリモートのアセットバンドルに依存しています");
                
                foreach (string assetPath in dependency.Value)
                {
                    // エントリ取得
                    AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));
                    // エントリに登録されてない
                    if(entry == null) continue;
                    // スキーマ取得
                    BundledAssetGroupSchema schema = entry.parentGroup.GetSchema<BundledAssetGroupSchema>();
                    // スキーマが存在しない
                    if(schema == null) continue;
                    // リモートグループ以外は無視
                    if(schema.BuildPath.GetName(settings) != AddressableAssetSettings.kRemoteBuildPath) continue;
                    
                    // 依存しているリモートアセット
                    count++;
                    sb.AppendLine("> " + assetPath);
                }
                
                // ログ
                if(count > 0)
                {
                    error = true;
                    UnityEngine.Object target = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dependency.Key);
                    Debug.LogError(sb.ToString(), target);
                }
            }
            
            EditorUtility.ClearProgressBar();
            
            if(error)
            {
                throw new Exception("ローカルアセットがリモートアセットへの参照を持ってます");
            }
        }
        
        [MenuItem(EditorMenu.AddressablesMenuPath + "/PlayModeScript/UseExistingBuild")]
        private static void UseExistingBuild()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (IDataBuilder builder in settings.DataBuilders)
            {
                if(builder.GetType() == typeof(BuildScriptPackedPlayMode))
                {
                    settings.ActivePlayModeDataBuilderIndex = settings.DataBuilders.IndexOf((ScriptableObject)builder);
                }
            }
        }

        [MenuItem(EditorMenu.AddressablesMenuPath + "/PlayModeScript/UseAssetDatabase")]
        private static void UseAssetDatabase()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (IDataBuilder builder in settings.DataBuilders)
            {
                if(builder.GetType() == typeof(BuildScriptFastMode))
                {
                    settings.ActivePlayModeDataBuilderIndex = settings.DataBuilders.IndexOf((ScriptableObject)builder);
                }
            }
        }
    }
}

#endif