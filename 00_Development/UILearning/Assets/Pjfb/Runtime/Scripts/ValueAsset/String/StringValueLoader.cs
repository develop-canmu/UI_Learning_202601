using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;

namespace Pjfb
{
    public class StringValueAssetLoader : ValueAssetLoader<StringValueAssetLoader, StringValueAsset, string>
    {
        public static readonly string LocalAddress = "ValueAsset/StringLocal.asset";
        public static readonly string RemoteAddress = "ValueAsset/StringRemote.asset";
        
        protected override string GetAddress()
        {
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return LocalAddress;
            }
            return RemoteAddress;
#else
            return LocalAddress;
#endif
        }
    }
}