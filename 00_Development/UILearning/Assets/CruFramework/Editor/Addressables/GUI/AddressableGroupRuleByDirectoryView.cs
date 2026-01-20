using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableGroupRuleByDirectoryView : AddressableGroupRuleView
    {
        private ObjectField directoryField = null;

        public AddressableGroupRuleByDirectoryView(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings) : base(addressableAssetSettings, addressableCustomSettings)
        {
            directoryField = new ObjectField();
            directoryField.label = "Directory";
            directoryField.objectType = typeof(DefaultAsset);
            Insert(3, directoryField);
        }

        public override void RegisterCallback(int index)
        {
            base.RegisterCallback(index);
            directoryField.UnregisterCallback<ChangeEvent<UnityEngine.Object>, int>(OnDirectoryChanged);
            directoryField.RegisterCallback<ChangeEvent<UnityEngine.Object>, int>(OnDirectoryChanged, index);
        }

        public override void UpdateView(int index)
        {
            base.UpdateView(index);
            AddressableGroupRuleByDirectory directoryRule = (AddressableGroupRuleByDirectory)AddressableCustomSettings.GroupRuleList[index]; 
            directoryField.value = directoryRule.Directory;
            PackingModeField.choices = AddressableDirectoryBundlePackingMode.All;
            PackingModeField.value = directoryRule.PackingMode;
        }

        private void OnDirectoryChanged(ChangeEvent<UnityEngine.Object> evt, int index)
        {
            AddressableUtility.SetDirty(AddressableCustomSettings, () => 
            {
                AddressableGroupRuleByDirectory directoryRule = (AddressableGroupRuleByDirectory)AddressableCustomSettings.GroupRuleList[index];
                directoryRule.Directory = (DefaultAsset)evt.newValue;
            });
        }
    }
}
