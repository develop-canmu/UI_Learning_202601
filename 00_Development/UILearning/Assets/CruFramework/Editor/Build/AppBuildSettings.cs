using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CruFramework.Editor.Build
{
    [CreateAssetMenu(fileName = nameof(AppBuildSettings), menuName = EditorMenu.BuildMenuPath + "/App/Create " + nameof(AppBuildSettings))]
    public class AppBuildSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("製品名")]
        private string productName = string.Empty;
        /// <summary>製品名</summary>
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }
        
        [SerializeField]
        [Tooltip("バンドルID")]
        private string bundleId = string.Empty;
        /// <summary>製品名</summary>
        public string BundleId
        {
            get { return bundleId; }
            set { bundleId = value; }
        }
        
        [SerializeField]
        [Tooltip("開発ビルド")]
        private bool developmentBuild = false;
        /// <summary>開発ビルドかどうか</summary>
        public bool DevelopmentBuild
        {
            get { return developmentBuild; }
            set { developmentBuild = value; }
        }

        [SerializeField]
        private List<string> defineSymbols = new List<string>();
        /// <summary>シンボル定義</summary>
        public List<string> DefineSymbols { get { return defineSymbols; } }

        [SerializeField]
        private List<string> scenes = new List<string>();
        /// <summary>ビルドシーン</summary>
        public List<string> Scenes { get { return scenes; } }
        
        [SerializeField]
        private List<DefaultAsset> excludedAsset = new List<DefaultAsset>();
        /// <summary>除外アセット（フォルダ指定も可能）</summary>
        public List<DefaultAsset> ExcludedAsset { get { return excludedAsset; } }

        [Header("iOS")]
        [SerializeField]
        [Tooltip("プロビジョニングプロファイルID")]
        private string provisioningProfileId = string.Empty;
        /// <summary>プロビジョニングプロファイルID</summary>
        public string ProvisioningProfileId
        {
            get { return provisioningProfileId; }
            set { provisioningProfileId = value; }
        }
        
        [SerializeField]
        [Tooltip("チームID")]
        private string signingTeamId = string.Empty;
        /// <summary>チームID</summary>
        public string SigningTeamId
        {
            get { return signingTeamId; }
            set { signingTeamId = value; }
        }
        
        [Header("Android")]
        [SerializeField]
        [Tooltip("キースストアファイル名")]
        private string keystoreName = string.Empty;
        /// <summary>キースストアファイルパス</summary>
        public string KeystoreName
        {
            get { return keystoreName; }
            set { keystoreName = value; }
        }
        
        [SerializeField]
        [Tooltip("キースストアパスワード")]
        private string keystorePass = string.Empty;
        /// <summary>キースストアパスワード</summary>
        public string KeystorePass
        {
            get { return keystorePass; }
            set { keystorePass = value; }
        }
        
        [SerializeField]
        [Tooltip("キースエイリアス名")]
        private string keyAliasName = string.Empty;
        /// <summary>キースエイリアス名</summary>
        public string KeyAliasName
        {
            get { return keyAliasName; }
            set { keyAliasName = value; }
        }
        
        [SerializeField]
        [Tooltip("キースエイリアスパスワード")]
        private string keyAliasPass = string.Empty;
        /// <summary>キースエイリアスパスワード</summary>
        public string KeyAliasPass
        {
            get { return keyAliasPass; }
            set { keyAliasPass = value; }
        }
    }
}
