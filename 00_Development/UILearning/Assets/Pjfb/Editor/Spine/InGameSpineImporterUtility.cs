using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Pjfb.Editor.Spine
{
    public class InGameSpineAtlasUtility
    {
        public static void ResolveInGameSpineMaterials()
        {
            var spineMatPathGUIDs = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Pjfb/Runtime/AssetBundles/Remote/Spines/InGame" });
            foreach (var spineMatPathGUID in spineMatPathGUIDs)
            {
                var curMatPath = AssetDatabase.GUIDToAssetPath(spineMatPathGUID);
                var curFolder = Path.GetDirectoryName(curMatPath);
                var texturePathGUIDs = AssetDatabase.FindAssets("t:Texture", new[] { $"{curFolder}" });
                foreach (var pathGUID in texturePathGUIDs)
                {
                    var curTexPath = AssetDatabase.GUIDToAssetPath(pathGUID);
                    var isValid = curTexPath.EndsWith("_red.png", StringComparison.Ordinal);
                    if (!isValid) continue;
                    
                    CruFramework.Logger.Log($"[SpineAtlasResolverUtility] Resolving {curTexPath}");
                    RemoveMaterialTexture(curMatPath);
                }
            }
        }
        
        private static void RemoveMaterialTexture(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (asset != null)
            {
                CruFramework.Logger.Log($"[SpineTextureChecker] Removing material texture for {path}");
                asset.mainTexture = null;
                AssetDatabase.SaveAssetIfDirty(asset);
            }
        }
    }
}