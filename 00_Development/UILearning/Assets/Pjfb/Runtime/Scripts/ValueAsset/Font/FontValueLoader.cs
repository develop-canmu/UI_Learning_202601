using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;
using CruFramework.Addressables;
using TMPro;

namespace Pjfb
{
    public class FontValueAssetLoader : ValueAssetLoader<FontValueAssetLoader, FontValueAsset, TMP_FontAsset>
    {
        protected override string GetAddress()
        {
            return "ValueAsset/Font.asset";
        }
    }
}