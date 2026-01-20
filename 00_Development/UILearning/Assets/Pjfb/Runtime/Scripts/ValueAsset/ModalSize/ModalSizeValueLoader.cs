using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework;
using CruFramework.Addressables;

namespace Pjfb
{
    public class ModalSizeValueAssetLoader : ValueAssetLoader<ModalSizeValueAssetLoader, ModalSizeValueAsset, Vector2>
    {
        protected override string GetAddress()
        {
            return "ValueAsset/ModalSize.asset";
        }
    }
}