using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
	public class AddressableImporter : AssetPostprocessor
	{
#if !DISABLE_ADDRESSABLE_IMPORTER

		public override uint GetVersion()
		{
			return 1;
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			// TODO 設定ファイルチェック
			
			AddressableAssetSettings assetSettings = AddressableUtility.GetAddressableAssetSettings();
			AddressableCustomSettingsObject customSettings = AddressableUtility.GetAddressableCustomSettings();

			for(int i = 0; i < importedAssets.Length; i++)
			{
				AddOrMoveAssetEntry(importedAssets[i], assetSettings, customSettings);
			}
			
			for(int i = 0; i < deletedAssets.Length; i++)
			{
				RemoveAssetEntry(deletedAssets[i], assetSettings);
			}

			for(int i = 0; i < movedAssets.Length; i++)
			{
				AddOrMoveAssetEntry(movedAssets[i], assetSettings, customSettings);
			}
			
		}
#endif
		
		/// <summary>エントリの登録</summary>
		internal static void AddOrMoveAssetEntry(string path, AddressableAssetSettings assetSettings, AddressableCustomSettingsObject customSettings)
		{
			// ディレクトリは無視
			if(IsDirectory(path)) return;
			// スクリプトは無視
			if(IsMonoScript(path)) return;

			// 対象ルール取得
			AddressableGroupRule groupRule = null;
			foreach (AddressableGroupRule rule in customSettings.GroupRuleList)
			{
				if(rule.IsMatch(path))
				{
					groupRule = rule;
					break;
				}
			}

			// ルールが存在しない
			if(groupRule == null)
			{
				RemoveAssetEntry(path, assetSettings);
				return;
			}

			// パックしない設定
			if(groupRule.PackingMode == AddressableBundlePackingMode.PackIgnore)
			{
				RemoveAssetEntry(path, assetSettings);
				return;
			}

			// グループ名
			string groupName = groupRule.GetGroupName(path);
			// グループ取得
			AddressableAssetGroup group = GetOrCreateAssetGroup(groupName, assetSettings);
			
			// バンドル設定
			BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
			AddressableBundledAssetSchemaSettings bundledAssetSchemaSettings = groupRule.OverrideBundledAssetGroupSchema ? groupRule.BundledAssetSchemaSettings : customSettings.DefaultBundledAssetSchemaSettings;
			// ビルドパス
			string buildPath = groupRule.GroupType == GroupType.Local ? AddressableAssetSettings.kLocalBuildPath : AddressableAssetSettings.kRemoteBuildPath;
			schema.BuildPath.SetVariableByName(assetSettings, buildPath);
			// ロードパス
			string loadPath = groupRule.GroupType == GroupType.Local ? AddressableAssetSettings.kLocalLoadPath : AddressableAssetSettings.kRemoteLoadPath;
			schema.LoadPath.SetVariableByName(assetSettings, loadPath);
			// パックモード
			schema.BundleMode = groupRule.PackingMode == AddressableBundlePackingMode.PackSeparately ? BundledAssetGroupSchema.BundlePackingMode.PackSeparately : BundledAssetGroupSchema.BundlePackingMode.PackTogether;
			// タイムアウト 
			schema.Timeout = bundledAssetSchemaSettings.RequestTimeout;
			// リダイレクト数
			schema.RedirectLimit = bundledAssetSchemaSettings.HttpRedirectLimit;
			// リトライ回数
			schema.RetryCount = bundledAssetSchemaSettings.RetryCount;
			// 圧縮設定
			schema.Compression = groupRule.GroupType == GroupType.Local ? BundledAssetGroupSchema.BundleCompressionMode.LZ4 : BundledAssetGroupSchema.BundleCompressionMode.LZMA;
			
			// TODO 
			// // カスタム設定
			// AddressableCustomGroupSchema customSchema = group.GetSchema<AddressableCustomGroupSchema>();
			// // 対象カタログ名
			// customSchema.CatalogName = groupRule.CatalogName;
			// // ローカルビルドに含めるか
			// customSchema.IncludeInBuildWhenPlayerBuild = groupRule.IncludeInBuildWhenPlayerBuild;

			// GUID
			string guid = AssetDatabase.AssetPathToGUID(path);
			// エントリ取得
			AddressableAssetEntry entry = assetSettings.FindAssetEntry(guid);
			if(entry != null)
			{
				// 現在のグループを取得
				AddressableAssetGroup currentGroup = entry.parentGroup;
				if(currentGroup != group)
				{
					// エントリを移動
					assetSettings.CreateOrMoveEntry(guid, group, true, false);
					// エントリが存在しなければグループを削除
					if(currentGroup.entries.Count <= 0)
					{
						assetSettings.RemoveGroup(currentGroup);
					}
				}
			}
			else
			{
				// エントリを生成する
				entry = assetSettings.CreateOrMoveEntry(guid, group, true, false);
			}
			
			string address = path;
			// アドレス置換
			foreach (AddressableAddressReplacement addressReplacement in customSettings.AddressReplacementList)
			{
				if(address.Contains(addressReplacement.SrcValue))
				{
					address = address.Replace(addressReplacement.SrcValue, addressReplacement.DestValue).Replace('\\', '/').Trim('/');
					break;
				}
			}
			// 拡張子を含めない
			if(!customSettings.IncludeExtensionInAddress)
			{
				address = Path.ChangeExtension(address, null);
			}
			// アドレス
			entry.SetAddress(address);
			
			// システムに登録されてるラベル一覧
			List<string> labelList = assetSettings.GetLabels();
			// ラベルを一度綺麗にする
			entry.labels.Clear();
			foreach (string label in groupRule.LabelList)
			{
				// システムに登録されてないものは追加しない
				if(!labelList.Contains(label)) return;
				// ラベル追加
				entry.SetLabel(label, true, false, false);
			}
		}
		
		/// <summary>エントリの登録解除</summary>
		internal static void RemoveAssetEntry(string path, AddressableAssetSettings assetSettings)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			// エントリ取得
			AddressableAssetEntry entry = assetSettings.FindAssetEntry(guid);
			AddressableAssetGroup currentGroup = null;
			if(entry != null)
			{
				// 現在のグループ
				currentGroup = entry.parentGroup;
			}
			// エントリ削除
			assetSettings.RemoveAssetEntry(guid, false);
			// エントリがなければグループ削除
			if(currentGroup != null)
			{
				if(currentGroup.entries.Count <= 0)
				{
					assetSettings.RemoveGroup(currentGroup);
				}
			}
		}
		
		private static AddressableAssetGroup GetOrCreateAssetGroup(string groupName, AddressableAssetSettings assetSettings)
		{
			AddressableAssetGroup group = assetSettings.FindGroup(groupName);
			// グループが存在しない
			if(group == null)
			{
				// スキーマ
				List<AddressableAssetGroupSchema> schemaList = new List<AddressableAssetGroupSchema>()
				{
					ScriptableObject.CreateInstance<ContentUpdateGroupSchema>(),
					ScriptableObject.CreateInstance<BundledAssetGroupSchema>(),
					ScriptableObject.CreateInstance<AddressableCustomGroupSchema>(),
				};
				// グループ生成
				group = assetSettings.CreateGroup(groupName, false, true, false, schemaList);
			}
			return group;
		}

		/// <summary>ディレクトリかどうか</summary>
		private static bool IsDirectory(string path)
		{
			if(!Directory.Exists(path)) return false;
			return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
		}
		
		/// <summary>スクリプトファイルかどうか</summary>
		private static bool IsMonoScript(string path)
		{
			return AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(MonoScript);
		}
	}
}
