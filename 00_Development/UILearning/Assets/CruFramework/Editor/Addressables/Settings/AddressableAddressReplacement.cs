using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [Serializable]
    public class AddressableAddressReplacement
    {
        [SerializeField]
        private string srcValue = string.Empty;
        public string SrcValue
        {
            get { return srcValue; }
            set { srcValue = value; }
        }
        
        [SerializeField]
        private string destValue = string.Empty;
        public string DestValue
        {
            get { return destValue; }
            set { destValue = value; }
        }
    }
}
