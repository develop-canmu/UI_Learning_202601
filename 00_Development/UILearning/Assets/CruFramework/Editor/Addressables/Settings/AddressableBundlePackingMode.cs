using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    public class AddressableBundlePackingMode
    {
        public const string PackTogether = "PackTogether";
        public const string PackSeparately = "PackSeparately";
        public const string PackIgnore = "PackIgnore";
        
        public static List<string> All
        {
            get 
            {
                return new List<string>()
                {
                    PackTogether,
                    PackSeparately,
                    PackIgnore
                };
            }
        }
    }
    
    public class AddressableDirectoryBundlePackingMode : AddressableBundlePackingMode
    {
        public const string PackTogetherByTopDirectory = "PackTogetherByTopDirectory";
        public const string PackTogetherByLastDirectory = "PackTogetherByLastDirectory";
        
        public static List<string> All
        {
            get
            {
                return new List<string>()
                {
                    PackTogether,
                    PackSeparately,
                    PackTogetherByTopDirectory,
                    PackTogetherByLastDirectory,
                    PackIgnore
                };
            }
        }
    }
}
