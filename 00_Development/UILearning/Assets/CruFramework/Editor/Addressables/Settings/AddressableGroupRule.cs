using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CruFramework.Editor.Addressable
{
    public enum GroupType
    {
        Local,
        Remote,
    }

    [Serializable]
    public abstract class AddressableGroupRule
    {
        [SerializeField]
        private string description = String.Empty;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        
        [SerializeField]
        private GroupType groupType = GroupType.Local;
        /// <summary>バンドルタイプ</summary>
        public GroupType GroupType
        {
            get { return groupType; }
            set { groupType = value; }
        }
        
        [SerializeField]
        private string packingMode = AddressableBundlePackingMode.PackTogether;
        /// <summary>パッキング方法</summary>
        public string PackingMode
        {
            get { return packingMode; }
            set { packingMode = value; }
        }
        
        [SerializeField]
        private string catalogName = string.Empty;
        public string CatalogName
        {
            get { return catalogName; }
            set { catalogName = value; }
        }
        
        [SerializeField]
        private List<string> labelList = new List<string>();
        /// <summary>ラベル</summary>
        public List<string> LabelList
        {
            get { return labelList; }
        }
        
        [SerializeField]
        private bool includeInBuildWhenPlayerBuild = false;
        public bool IncludeInBuildWhenPlayerBuild
        {
            get { return includeInBuildWhenPlayerBuild; }
            set { includeInBuildWhenPlayerBuild = value; }
        }
        
        [SerializeField]
        private bool overrideBundledAssetGroupSchema = false;
        /// <summary>コンテンツ更新設定を上書きするか</summary>
        public bool OverrideBundledAssetGroupSchema
        {
            get { return overrideBundledAssetGroupSchema; }
            set { overrideBundledAssetGroupSchema = value; }
        }
        
        [SerializeField]
        private AddressableBundledAssetSchemaSettings bundledAssetSchemaSettings = new AddressableBundledAssetSchemaSettings();
        /// <summary>コンテンツ更新設定</summary>
        public AddressableBundledAssetSchemaSettings BundledAssetSchemaSettings
        {
            get { return bundledAssetSchemaSettings; }
        }
        
        /// <summary>対象かどうか</summary>
        public abstract bool IsMatch(string path);
        
        /// <summary>グループ名の取得</summary>
        public abstract string GetGroupName(string path);
        
        protected string ReplacePathToGroupName(string path)
        {
            return path.Replace('\\', '/').Replace('/', '.');
        }
    }
}
