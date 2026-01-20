using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;
using CruFramework.Addressables;
using TMPro;

namespace Pjfb
{
    public class FontSizeValueAssetLoader : ValueAssetLoader<FontSizeValueAssetLoader, FontSizeValueAsset, float>
    {
        public static readonly string LocalAddress = "ValueAsset/FontSizeLocal.asset";
        public static readonly string RemoteAddress = "ValueAsset/FontSizeRemote.asset";
        
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