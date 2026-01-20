using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class CharacterStatusValueView : MonoBehaviour
    {
        public enum ColorType
        {
            Normal,
            After
        }
        
        [SerializeField]
        private TMPro.TMP_Text valueText;
        [SerializeField]
        private OmissionTextSetter omissionTextSetter;
        [SerializeField]
        private CharacterStatusRankImage rankImage;
        [SerializeField] 
        private CharacterStatusTypeImage statusTypeImage;
        [SerializeField] [ColorValue] 
        private string NormalColorId = string.Empty;
        [SerializeField] [ColorValue] 
        private string AfterColorId = string.Empty;

        
        public void SetValue(BigValue value)
        {
            if (omissionTextSetter == null)
            {
                valueText.text = value.ToString();
            }
            else
            {
                valueText.text = value.ToDisplayString(omissionTextSetter.GetOmissionData());
            }
            
            long rank = StatusUtility.GetRank(CharaRankMasterStatusType.Status, value);
            if(rankImage != null)
                rankImage.SetTexture(rank);
        }
        
        public void SetValue(string value)
        {
            valueText.text = value;
        }

        public void SetTypeImage(CharacterStatusType type)
        {
            if(statusTypeImage)
                statusTypeImage.SetTexture(type);
        }

        public void SetColor(bool isLevelUp)
        {
            valueText.color = isLevelUp ? GetTextColor(ColorType.After) : GetTextColor(ColorType.Normal);
        }
        
        private Color GetTextColor(ColorType type)
        {
            Color color = default;
            switch (type)
            {
                case ColorType.Normal:
                    if(ColorValueAssetLoader.Instance.HasKey(NormalColorId))
                    {
                        color = ColorValueAssetLoader.Instance[NormalColorId];
                    }
                    break;
                case ColorType.After:
                    if(ColorValueAssetLoader.Instance.HasKey(AfterColorId))
                    {
                        color = ColorValueAssetLoader.Instance[AfterColorId];
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return color;
        }
    }
}
