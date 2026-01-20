using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
	public class AddressableBuildMenuView : AddressableFoldoutHeaderView
	{
		private AddressableAssetSettings addressableAssetSettings = null;
		
		private Toggle ignoreUnsupportedFilesInBuildField = null;
		private Toggle uniqueBundleIdsField = null;
		private Toggle contiguousBundlesField = null;
		private Toggle stripUnityVersionFromBundleBuildField = null;
		private Toggle disableVisibleSubAssetRepresentationsField = null;

		public AddressableBuildMenuView(AddressableAssetSettings addressableAssetSettings)
		{
			this.addressableAssetSettings = addressableAssetSettings;
			
			text = "Build";
			
			ignoreUnsupportedFilesInBuildField = new Toggle();
			ignoreUnsupportedFilesInBuildField.label = "Ignore Invalid/Unsupported Files in Build";
			// 非対象ファイルを無視してビルドを停止させないようにする
			ignoreUnsupportedFilesInBuildField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.IgnoreUnsupportedFilesInBuild = evt.newValue);
			});
			Add(ignoreUnsupportedFilesInBuildField);
			
			uniqueBundleIdsField = new Toggle();
			uniqueBundleIdsField.label = "Unique Bundle IDs";
			uniqueBundleIdsField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.UniqueBundleIds = evt.newValue);
			});
			Add(uniqueBundleIdsField);
			
			contiguousBundlesField = new Toggle();
			contiguousBundlesField.label = "Contiguous Bundles";
			contiguousBundlesField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.ContiguousBundles = evt.newValue);
			});
			Add(contiguousBundlesField);
			
			stripUnityVersionFromBundleBuildField = new Toggle();
			stripUnityVersionFromBundleBuildField.label = "Strip Unity Version from AssetBundles";
			stripUnityVersionFromBundleBuildField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(addressableAssetSettings, () => StripUnityVersionFromBundleBuildInfo.SetValue(addressableAssetSettings, evt.newValue));
			});
			Add(stripUnityVersionFromBundleBuildField);
			
			disableVisibleSubAssetRepresentationsField = new Toggle();
			disableVisibleSubAssetRepresentationsField.label = "Disable Visible Sub Asset Representations";
			disableVisibleSubAssetRepresentationsField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.DisableVisibleSubAssetRepresentations = evt.newValue);
			});
			Add(disableVisibleSubAssetRepresentationsField);
		}

		public override void UpdateView()
		{
			ignoreUnsupportedFilesInBuildField.value = addressableAssetSettings.IgnoreUnsupportedFilesInBuild;
			uniqueBundleIdsField.value = addressableAssetSettings.UniqueBundleIds;
			contiguousBundlesField.value = addressableAssetSettings.ContiguousBundles;
			stripUnityVersionFromBundleBuildField.value = (bool)StripUnityVersionFromBundleBuildInfo.GetValue(addressableAssetSettings);
			disableVisibleSubAssetRepresentationsField.value = addressableAssetSettings.DisableVisibleSubAssetRepresentations;
		}
		
		private FieldInfo StripUnityVersionFromBundleBuildInfo
		{
			get { return addressableAssetSettings.GetType().GetField("m_StripUnityVersionFromBundleBuild", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); }
		}
	}
}
