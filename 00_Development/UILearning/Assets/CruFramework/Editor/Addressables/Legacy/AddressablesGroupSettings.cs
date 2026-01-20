#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressables
{
    /// <summary>グループタイプ</summary>
    public enum AddressablesGroupType
    {
        Local,
        Remote
    }
    
    /// <summary>アセットバンドルのパッキング方法</summary>
    public enum AddressablesBundlePackingMode
    {
        PackTogether,
        PackTogetherByTopDirectory,
        PackSeparately
    }
    
    /// <summary>アドレスのルートパス指定</summary>
    public enum AddressablesAddressRoot
    {
        ProjectRoot,
        CurrentDirectory,
    }
    
    [CreateAssetMenu(fileName = nameof(AddressablesGroupSettings), menuName = EditorMenu.AddressablesMenuPath + "/Create " + nameof(AddressablesGroupSettings))]
    public class AddressablesGroupSettings : ScriptableObject
    {
        [SerializeField]
        private AddressablesGroupType groupType = AddressablesGroupType.Local;
        /// <summary>グループタイプ</summary>
        public AddressablesGroupType GroupType
        {
            get { return groupType; }
            set { groupType = value; }
        }

        [SerializeField]
        private AddressablesBundlePackingMode packingMode = AddressablesBundlePackingMode.PackTogether;
        /// <summary>パッキングする単位</summary>
        public AddressablesBundlePackingMode PackingMode
        {
            get { return packingMode; }
            set { packingMode = value; }
        }

        [SerializeField]
        private AddressablesAddressRoot addressRoot = AddressablesAddressRoot.ProjectRoot;
        /// <summary>アドレスのルートパス指定</summary>
        public AddressablesAddressRoot AddressRoot
        {
            get { return addressRoot; }
            set { addressRoot = value; }
        }

        [SerializeField]
        private string addressRootOverride = string.Empty;
        /// <summary>アドレスのルートパスの上書き</summary>
        public string AddressRootOverride
        {
            get { return addressRootOverride; }
            set { addressRootOverride = value; }
        }
        
        [SerializeField]
        private int requestTimeout = 0;
        /// <summary>タイムアウト間隔</summary>
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        [SerializeField]
        private int httpRedirectLimit = 0;
        /// <summary>リダイレクト数</summary>
        public int HttpRedirectLimit
        {
            get { return httpRedirectLimit; }
            set { httpRedirectLimit = value; }
        }

        [SerializeField]
        private int retryCount = 0;
        /// <summary>リトライカウント</summary>
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        [SerializeField]
        private List<string> labels = new List<string>();
        /// <summary>ラベル</summary>
        public List<string> Labels { get { return labels; } }
    }
}

#endif