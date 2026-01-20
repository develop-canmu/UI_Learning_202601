#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace CruFramework.EditorExtension.Addressables
{
    public static class AddressablesUtility
    {
        /// <summary>アドレスからアセットパスを取得</summary>
        public static string GetAssetPathFromAddress(string address)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (AddressableAssetGroup group in settings.groups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    if(entry.address == address)
                    {
                        return entry.AssetPath;
                    }
                }
            }
            return null;
        }
    }
}

#endif