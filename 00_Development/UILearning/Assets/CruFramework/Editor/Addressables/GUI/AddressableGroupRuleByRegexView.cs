using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableGroupRuleByRegexView : AddressableGroupRuleView
    {
        private EnumField matchTypeField = null;
        private TextField pathRegexField = null;
        private TextField groupNameField = null;
        
        public AddressableGroupRuleByRegexView(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings) : base(addressableAssetSettings, addressableCustomSettings)
        {
            matchTypeField = new EnumField();
            matchTypeField.label = "Match Type";
            matchTypeField.Init(AddressableGroupRuleByRegex.RegexMatchType.Wildcard);
            Insert(3, matchTypeField);
            
            pathRegexField = new TextField();
            pathRegexField.label = "Path Regex";
            Insert(4, pathRegexField);
            
            groupNameField = new TextField();
            groupNameField.label = "Group Name";
            Insert(5, groupNameField);
        }

        public override void RegisterCallback(int index)
        {
            base.RegisterCallback(index);
            matchTypeField.UnregisterCallback<ChangeEvent<Enum>, int>(OnMatchTypeChanged);
            matchTypeField.RegisterCallback<ChangeEvent<Enum>, int>(OnMatchTypeChanged, index);
            
            pathRegexField.UnregisterCallback<ChangeEvent<string>, int>(OnPathRegexChanged);
            pathRegexField.RegisterCallback<ChangeEvent<string>, int>(OnPathRegexChanged, index);
            
            groupNameField.UnregisterCallback<ChangeEvent<string>, int>(OnGroupNameChanged);
            groupNameField.RegisterCallback<ChangeEvent<string>, int>(OnGroupNameChanged, index);
        }

        public override void UpdateView(int index)
        {
            base.UpdateView(index);
            AddressableGroupRuleByRegex regexRule = (AddressableGroupRuleByRegex)AddressableCustomSettings.GroupRuleList[index];
            matchTypeField.value = regexRule.MatchType;
            pathRegexField.value = regexRule.PathRegex;
            groupNameField.value = regexRule.GroupName;
            PackingModeField.choices = AddressableBundlePackingMode.All;
            PackingModeField.value = regexRule.PackingMode;
        }
        
        private void OnMatchTypeChanged(ChangeEvent<Enum> evt, int index)
        {
            AddressableUtility.SetDirty(AddressableCustomSettings, () =>
            {
                AddressableGroupRuleByRegex regexRule = (AddressableGroupRuleByRegex)AddressableCustomSettings.GroupRuleList[index];
                regexRule.MatchType = (AddressableGroupRuleByRegex.RegexMatchType)evt.newValue;
            });
        }
        
        private void OnPathRegexChanged(ChangeEvent<string> evt, int index)
        {
            AddressableUtility.SetDirty(AddressableCustomSettings, () =>
            {
                AddressableGroupRuleByRegex regexRule = (AddressableGroupRuleByRegex)AddressableCustomSettings.GroupRuleList[index];
                regexRule.PathRegex = evt.newValue;
            });
        }
        
        private void OnGroupNameChanged(ChangeEvent<string> evt, int index)
        {
            AddressableUtility.SetDirty(AddressableCustomSettings, () =>
            {
                AddressableGroupRuleByRegex regexRule = (AddressableGroupRuleByRegex)AddressableCustomSettings.GroupRuleList[index];
                regexRule.GroupName = evt.newValue;
            });
        }
    }
}
