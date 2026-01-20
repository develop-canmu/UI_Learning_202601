using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
	public abstract class AddressableGroupRuleView : VisualElement
	{
		private AddressableAssetSettings addressableAssetSettings = null;
		protected AddressableAssetSettings AddressableAssetSettings
		{
			get { return addressableAssetSettings; }
		}
		
		private AddressableCustomSettingsObject addressableCustomSettings = null;
		protected AddressableCustomSettingsObject AddressableCustomSettings
		{
			get { return addressableCustomSettings; }
		}
		
		private TextField descriptionField = null;
		private EnumField groupTypeField = null;
		private PopupField<string> catalogNameField = null;
		private PopupField<string> packingModeField = null;
		protected PopupField<string> PackingModeField
		{
			get { return packingModeField; }
		}
		private MaskField labelField = null;
		private Toggle includeInBuildWhenPlayerBuildField = null;
		private Toggle overrideBundledAssetGroupSchemaField = null;
		private AddressableBundledAssetSchemaSettingsView bundledAssetSchemaSettingsView = null;
		
		public AddressableGroupRuleView(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings)
		{
			this.addressableAssetSettings = addressableAssetSettings;
			this.addressableCustomSettings = addressableCustomSettings;
			
			descriptionField = new TextField();
			Add(descriptionField);
			
			groupTypeField = new EnumField();
			groupTypeField.label = "Bundle Type";
			groupTypeField.Init(GroupType.Local);
			Add(groupTypeField);
			
			catalogNameField = new PopupField<string>();
			catalogNameField.label = "Catalog Name";
			Add(catalogNameField);
			
			packingModeField = new PopupField<string>();
			packingModeField.label = "Packing Mode";
			Add(packingModeField);
			
			labelField = new MaskField();
			labelField.label = "Labels";
			Add(labelField);
			
			includeInBuildWhenPlayerBuildField = new Toggle();
			includeInBuildWhenPlayerBuildField.label = "Include In Build When Player Build";
			Add(includeInBuildWhenPlayerBuildField);

			overrideBundledAssetGroupSchemaField = new Toggle();
			overrideBundledAssetGroupSchemaField.label = "Override Bundled Asset Schema";
			Add(overrideBundledAssetGroupSchemaField);
			
			bundledAssetSchemaSettingsView = new AddressableBundledAssetSchemaSettingsView(addressableCustomSettings);
			Add(bundledAssetSchemaSettingsView);
		}
		
		public virtual void RegisterCallback(int index)
		{
			descriptionField.UnregisterCallback<ChangeEvent<string>, int>(OnDescriptionChanged);
			descriptionField.RegisterCallback<ChangeEvent<string>, int>(OnDescriptionChanged, index);
			
			groupTypeField.UnregisterCallback<ChangeEvent<Enum>, int>(OnBundleTypeChanged);
			groupTypeField.RegisterCallback<ChangeEvent<Enum>, int>(OnBundleTypeChanged, index);
			
			catalogNameField.UnregisterCallback<ChangeEvent<string>, int>(OnCatalogNameChanged);
			catalogNameField.RegisterCallback<ChangeEvent<string>, int>(OnCatalogNameChanged, index);
			
			packingModeField.UnregisterCallback<ChangeEvent<string>, int>(OnPackingModeChanged);
			packingModeField.RegisterCallback<ChangeEvent<string>, int>(OnPackingModeChanged, index);
			
			includeInBuildWhenPlayerBuildField.UnregisterCallback<ChangeEvent<bool>, int>(OnIncludeInBuildWhenPlayerBuildChanged);
			includeInBuildWhenPlayerBuildField.RegisterCallback<ChangeEvent<bool>, int>(OnIncludeInBuildWhenPlayerBuildChanged, index);
			
			overrideBundledAssetGroupSchemaField.UnregisterCallback<ChangeEvent<bool>, int>(OnOverrideBundledAssetGroupSchemaChanged);
			overrideBundledAssetGroupSchemaField.RegisterCallback<ChangeEvent<bool>, int>(OnOverrideBundledAssetGroupSchemaChanged, index);
			
			bundledAssetSchemaSettingsView.UnregisterCallback();
			bundledAssetSchemaSettingsView.RegisterCallback(addressableCustomSettings.GroupRuleList[index].BundledAssetSchemaSettings);
			
			labelField.UnregisterCallback<ChangeEvent<int>, int>(OnLabelChanged);
			labelField.RegisterCallback<ChangeEvent<int>, int>(OnLabelChanged, index);
		}
		
		public virtual void UpdateView(int index)
		{
			AddressableGroupRule rule = addressableCustomSettings.GroupRuleList[index];
			descriptionField.value = rule.Description;
			groupTypeField.value = rule.GroupType;
			catalogNameField.choices = addressableCustomSettings.CatalogSettingsList.Select(v => v.CatalogName).ToList();
			catalogNameField.value = rule.CatalogName;
			includeInBuildWhenPlayerBuildField.style.display = rule.GroupType == GroupType.Remote ? DisplayStyle.Flex : DisplayStyle.None;
			includeInBuildWhenPlayerBuildField.value = rule.IncludeInBuildWhenPlayerBuild;
			overrideBundledAssetGroupSchemaField.value = rule.OverrideBundledAssetGroupSchema;
			bundledAssetSchemaSettingsView.style.display = rule.OverrideBundledAssetGroupSchema ? DisplayStyle.Flex : DisplayStyle.None;
			bundledAssetSchemaSettingsView.UpdateView(rule.BundledAssetSchemaSettings);

			// ラベル
			List<string> labels = addressableAssetSettings.GetLabels();
			labelField.choices = labels;
			int value = 0;
			for(int i = 0; i < labels.Count; i++)
			{
				// ラベルが登録されている
				if(rule.LabelList.Contains(labels[i]))
				{
					// インデックスを左シフトしてORで有効化
					value = value | (1 << i);
				}
			}
			labelField.value = value;
		}
		
		private void OnDescriptionChanged(ChangeEvent<string> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList[index].Description = evt.newValue);
		}
		
		private void OnBundleTypeChanged(ChangeEvent<Enum> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () => 
			{
				AddressableGroupRule rule = addressableCustomSettings.GroupRuleList[index];
				rule.GroupType = (GroupType)evt.newValue;
				includeInBuildWhenPlayerBuildField.style.display = rule.GroupType == GroupType.Remote ? DisplayStyle.Flex : DisplayStyle.None;
			});
		}

		private void OnCatalogNameChanged(ChangeEvent<string> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList[index].CatalogName = evt.newValue);
		}
		
		private void OnPackingModeChanged(ChangeEvent<string> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList[index].PackingMode = evt.newValue);
		}
		
		private void OnIncludeInBuildWhenPlayerBuildChanged(ChangeEvent<bool> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList[index].IncludeInBuildWhenPlayerBuild = evt.newValue);
		}

		private void OnOverrideBundledAssetGroupSchemaChanged(ChangeEvent<bool> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () =>
			{
				AddressableGroupRule rule = addressableCustomSettings.GroupRuleList[index];
				rule.OverrideBundledAssetGroupSchema = evt.newValue;
				bundledAssetSchemaSettingsView.style.display = rule.OverrideBundledAssetGroupSchema ? DisplayStyle.Flex : DisplayStyle.None;
			});
		}

		private void OnLabelChanged(ChangeEvent<int> evt, int index)
		{
			AddressableUtility.SetDirty(addressableCustomSettings, () =>
			{
				// 一度綺麗にする
				addressableCustomSettings.GroupRuleList[index].LabelList.Clear();
				// ラベル一覧
				List<string> labels = addressableAssetSettings.GetLabels();
				for(int i = 0; i < labels.Count; i++)
				{
					// 選択されてるかチェック
					if((evt.newValue & (1 << i)) != 0)
					{
						// ラベルリストに追加
						addressableCustomSettings.GroupRuleList[index].LabelList.Add(labels[i]);
					}
				}
			});
		}
	}
}
