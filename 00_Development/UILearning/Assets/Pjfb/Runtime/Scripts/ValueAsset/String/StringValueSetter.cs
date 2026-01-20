using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Pjfb
{
    public class StringValueSetter : MonoBehaviour
    {
        [SerializeField]
        [FontValue]
        private string fontId = "default";
        
        [SerializeField]
        [FontSizeValue]
        private string fontSizeId = "default";
        
        [SerializeField]
        private bool isFontSizeAutoResize = false;
        
        [SerializeField]
        [ColorValue]
        private string colorId = string.Empty;
        
        [SerializeField]
        [StringValue]
        private string stringId = string.Empty;
        /// <summary>StringId</summary>
        public string StringId{get{return stringId;}}
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateText();
        }
#endif
        
        private void UpdateText()
        {
            TMP_Text text = gameObject.GetComponent<TMP_Text>();
            if(text != null)
            {
                if(FontValueAssetLoader.Instance.HasKey(fontId))
                {
                    text.font = FontValueAssetLoader.Instance[fontId];
                }
                
                if(FontSizeValueAssetLoader.Instance.HasKey(fontSizeId))
                {
                    text.fontSize = FontSizeValueAssetLoader.Instance[fontSizeId];
                    text.enableAutoSizing = isFontSizeAutoResize;
                    if(isFontSizeAutoResize)
                    {
                        text.fontSizeMin = text.fontSize;
                        text.fontSizeMax = text.fontSize;
                        text.characterWidthAdjustment = 50;
                    }
                }
                
                if(ColorValueAssetLoader.Instance.HasKey(colorId))
                {
                    text.color = ColorValueAssetLoader.Instance[colorId];
                }
                
                if(StringValueAssetLoader.Instance.HasKey(stringId))
                {
                    text.text = StringValueAssetLoader.Instance[stringId];
                }
            }
        }
        
        private void Start()
        {
            UpdateText();
        }
    }
}