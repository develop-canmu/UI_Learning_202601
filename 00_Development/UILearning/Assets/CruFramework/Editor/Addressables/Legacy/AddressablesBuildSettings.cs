#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressables
{
    [CreateAssetMenu(fileName = nameof(AddressablesBuildSettings), menuName = EditorMenu.BuildMenuPath + "/Addressables/Create " + nameof(AddressablesBuildSettings))]
    public class AddressablesBuildSettings : ScriptableObject
    {
        public enum CatalogNameType
        {
            Version,
            String,    
        }
        
        [SerializeField]
        private string remoteBuildPath = "ServerData/[BuildTarget]/[AssetVersion]";
        /// <summary>ビルドパス</summary>
        public string RemoteBuildPath { get { return remoteBuildPath; } }

        [SerializeField]
        private string remoteLoadPath = "{Namespace.Class.Property}/[BuildTarget]/[AssetVersion]";
        /// <summary>ロードパス</summary>
        public string RemoteLoadPath { get { return remoteLoadPath; } }
        
        [SerializeField]
        private string profileName = "Default";
        /// <summary>プロファイル名</summary>
        public string ProfileName { get { return profileName; }}
        
        [SerializeField]
        private CatalogNameType catalogNaming = CatalogNameType.String;
        /// <summary>カタログ命名</summary>
        public CatalogNameType CatalogNaming { get { return catalogNaming; } }
        
        [SerializeField]
        private string overridePlayerVersion = string.Empty;
        /// <summary>カタログ名</summary>
        public string OverridePlayerVersion { get { return overridePlayerVersion; } }
    }
}

#endif