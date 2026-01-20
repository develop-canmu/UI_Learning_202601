using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CruFramework.Editor.Addressable
{
    public class AddressableCustomSettingsObject : ScriptableObject
    {
        [SerializeField]
        private bool includeExtensionInAddress = true;
        /// <summary>アドレスに拡張子を含めるか</summary>
        public bool IncludeExtensionInAddress
        {
            get { return includeExtensionInAddress; }
            set { includeExtensionInAddress = value; }
        }
        
        [SerializeField]
        private List<AddressableAddressReplacement> addressReplacementList = new List<AddressableAddressReplacement>();
        /// <summary>除外パス一覧</summary>
        public List<AddressableAddressReplacement> AddressReplacementList
        {
            get { return addressReplacementList; }
        }
        
        [SerializeField]
        private List<AddressableCatalogSettings> catalogSettingsList = new List<AddressableCatalogSettings>();
        public List<AddressableCatalogSettings> CatalogSettingsList
        {
            get { return catalogSettingsList; }
        }

        [SerializeField]
        private AddressableBundledAssetSchemaSettings defaultBundledAssetSchemaSettings = new AddressableBundledAssetSchemaSettings();
        /// <summary>コンテンツ更新のデフォルト設定</summary>
        public AddressableBundledAssetSchemaSettings DefaultBundledAssetSchemaSettings
        {
            get { return defaultBundledAssetSchemaSettings; }
        }

        [SerializeReference]
        private List<AddressableGroupRule> groupRuleList = new List<AddressableGroupRule>();
        /// <summary>グループ設定一覧</summary>
        public List<AddressableGroupRule> GroupRuleList
        {
            get { return groupRuleList; }
        }
    }
}
