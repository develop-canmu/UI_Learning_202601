using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pjfb.InGame
{
    [System.Serializable]
    public class MaterialListWrapper
    {
        public List<Material> materials;
    }
    
    public class ModelMaterialSetter : MonoBehaviour
    {
        [SerializeField] private List<SkinnedMeshRenderer> renderers;
        [SerializeField] private List<MaterialListWrapper> opaqueMaterials;
        [SerializeField] private List<MaterialListWrapper> transparentMaterials;

        private List<Material[]> createdOpaqueMaterials;
        private List<Material[]> createdTransparentMaterials;

        public void SetOpaqueMaterial()
        {
            if (createdOpaqueMaterials != null)
            {
                for (var i = 0; i < renderers.Count; i++)
                {
                    renderers[i].materials = createdOpaqueMaterials[i];
                }

                return;
            }

            createdOpaqueMaterials = new List<Material[]>();
            for (var i = 0; i < renderers.Count; i++)
            {
                createdOpaqueMaterials.Add(new Material[renderers[i].sharedMaterials.Length]);
                for (var j = 0; j < renderers[i].sharedMaterials.Length ; j++)
                {
                    createdOpaqueMaterials[i][j] = opaqueMaterials[i].materials[j];
                }
                renderers[i].materials = createdOpaqueMaterials[i];
            }
        }
        
        public void SetTransparentMaterial()
        {
            if (createdTransparentMaterials != null)
            {
                for (var i = 0; i < renderers.Count; i++)
                {
                    renderers[i].materials = createdTransparentMaterials[i];
                }

                return;
            }

            createdTransparentMaterials = new List<Material[]>();
            for (var i = 0; i < renderers.Count; i++)
            {
                createdTransparentMaterials.Add(new Material[renderers[i].sharedMaterials.Length]);
                for (var j = 0; j < renderers[i].sharedMaterials.Length ; j++)
                {
                    createdTransparentMaterials[i][j] = transparentMaterials[i].materials[j];
                }
                renderers[i].materials = createdTransparentMaterials[i];
            }
        }
#if UNITY_EDITOR
        /// <summary>
        /// editor only event.
        /// </summary>
        private void Reset()
        {
            var meshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            renderers = new List<SkinnedMeshRenderer>();
            opaqueMaterials = new List<MaterialListWrapper>();
            transparentMaterials = new List<MaterialListWrapper>();

            for (var i = 0; i < meshRenderers.Length; i++)
            {
                renderers.Add(meshRenderers[i]);

                opaqueMaterials.Add(new MaterialListWrapper());
                opaqueMaterials[i].materials = new List<Material>();
                
                transparentMaterials.Add(new MaterialListWrapper());
                transparentMaterials[i].materials = new List<Material>();
                
                for (var j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    opaqueMaterials[i].materials.Add(renderers[i].sharedMaterials[j]);

                    var transparentMaterialPGuid= AssetDatabase.FindAssets(renderers[i].sharedMaterials[j].name + "_transparent").FirstOrDefault();
                    if (!string.IsNullOrEmpty(transparentMaterialPGuid))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(transparentMaterialPGuid);
                        transparentMaterials[i].materials.Add(AssetDatabase.LoadAssetAtPath<Material>(path));
                    }
                    else
                    {
                        transparentMaterialPGuid= AssetDatabase.FindAssets(renderers[i].sharedMaterials[j].name).FirstOrDefault();
                        var path = AssetDatabase.GUIDToAssetPath(transparentMaterialPGuid);
                        var newMaterialPath = path.Replace(".mat", "_transparent.mat");
                        AssetDatabase.CopyAsset(path, newMaterialPath);
                        transparentMaterials[i].materials.Add(AssetDatabase.LoadAssetAtPath<Material>(newMaterialPath));
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
#endif
    }
}