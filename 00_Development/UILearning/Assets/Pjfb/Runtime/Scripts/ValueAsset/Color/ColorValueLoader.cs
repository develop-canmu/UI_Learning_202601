using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;
using CruFramework.Addressables;

namespace Pjfb
{
    public class ColorValueAssetLoader : ValueAssetLoader<ColorValueAssetLoader, ColorValueAsset, Color>
    {
        public static readonly string LocalAddress = "ValueAsset/ColorLocal.asset";
        public static readonly string RemoteAddress = "ValueAsset/ColorRemote.asset";
        
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