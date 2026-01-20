using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CruFramework.ResourceManagement;
using TMPro;
using UnityEngine;
namespace Pjfb.Runtime.Scripts.Utility
{
    public class DynamicAssetFontUtility : MonoBehaviour
    {
        public static DynamicAssetFontUtility Instance { get; private set; }

        [SerializeField] private TextAsset activeCharacterTable;

        [SerializeField]
        [FontValue]
        private string[] fontIdList = null;

        private List<TMP_FontAsset> _registeredAssetFonts = new List<TMP_FontAsset>();

        private async void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        
            //Load local font asset from addressable
            foreach (var fontId in fontIdList)
            {
                _registeredAssetFonts.Add(FontValueAssetLoader.Instance[fontId]);
            }
        }

        public bool HasCharacter(char c)
        {
            if (activeCharacterTable == null) return false;
            return activeCharacterTable.text.Contains(c);
        }

        private void OnDestroy()
        {
            _registeredAssetFonts.Clear();
        }

        public void ResetFontData()
        {
            foreach (var asset in _registeredAssetFonts)
            {
                CruFramework.Logger.Log($"[TMP_FontAsset]{asset.name} pre-clear atlasCount:{asset.atlasTextures.Length}");
                asset.ClearFontAssetData(true);
                CruFramework.Logger.Log($"[TMP_FontAsset]{asset.name} post-clear atlasCount:{asset.atlasTextures.Length}");
            }
        }
    }
}
