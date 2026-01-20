using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableGroupRuleViewContainer : VisualElement
    {
        private AddressableGroupRuleByDirectoryView directoryRuleView = null;
        public AddressableGroupRuleByDirectoryView DirectoryRuleView
        {
            get { return directoryRuleView; }
        }
        
        private AddressableGroupRuleByRegexView regexRuleView = null;
        public AddressableGroupRuleByRegexView RegexRuleView
        {
            get { return regexRuleView; }
        }
        
        public AddressableGroupRuleViewContainer(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings)
        {
            directoryRuleView = new AddressableGroupRuleByDirectoryView(addressableAssetSettings, addressableCustomSettings);
            Add(directoryRuleView);
            
            regexRuleView = new AddressableGroupRuleByRegexView(addressableAssetSettings, addressableCustomSettings);
            Add(regexRuleView);
        }
    }
}
