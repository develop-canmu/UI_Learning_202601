using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [DisplayName("AddressableCustomGroupSchema")]
    public class AddressableCustomGroupSchema : AddressableAssetGroupSchema
    {
        [SerializeField]
        private string catalogName = string.Empty;
        /// <summary>対象カタログ名</summary>
        public string CatalogName
        {
            get { return catalogName; }
            set
            {
                catalogName = value;
                SetDirty(true);
            }
        }
        
        [SerializeField]
        private bool includeInBuildWhenPlayerBuild = false;
        /// <summary>アプリビルド時にリモートグループをビルド対象に含めるかどうか</summary>
        public bool IncludeInBuildWhenPlayerBuild
        {
            get { return includeInBuildWhenPlayerBuild; }
            set
            {
                includeInBuildWhenPlayerBuild = value;
                SetDirty(true);
            }
        }
    }
}
