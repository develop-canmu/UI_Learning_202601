using System;
using UnityEngine;

namespace Pjfb.Character
{
    public class PracticeSkillView : MonoBehaviour
    {
        public enum ColorType
        {
            Normal,
            After
        }
        
        [SerializeField] private TMPro.TMP_Text trainingContentText = null;
        [SerializeField] private TMPro.TMP_Text valueText = null;
        [SerializeField] private Color textNormalColor;
        [SerializeField] private Color textAfterColor;
        [SerializeField] private GameObject lockRoot;

        public void SetPracticeSkill(string content, string value)
        {
            trainingContentText.text = content;
            if (string.IsNullOrEmpty(value))
            {
                valueText.gameObject.SetActive(false);
                return;
            } 
            valueText.text = value;
        }

        public void SetContentTextColor(ColorType type)
        {
            Color color;
            switch (type)
            {
                case ColorType.Normal:
                    color = textNormalColor;
                    break;
                case ColorType.After:
                    color = textAfterColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            trainingContentText.color = color;
        }
        
        public void SetValueTextColor(ColorType type)
        {
            Color color;
            switch (type)
            {
                case ColorType.Normal:
                    color = textNormalColor;
                    break;
                case ColorType.After:
                    color = textAfterColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            valueText.color = color;
        }

        public void SetLock(bool isLock)
        {
            lockRoot.SetActive(isLock);
        }
    }
}