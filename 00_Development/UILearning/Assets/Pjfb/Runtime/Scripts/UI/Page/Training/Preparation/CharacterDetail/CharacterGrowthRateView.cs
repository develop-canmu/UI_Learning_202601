using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class CharacterGrowthRateView : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text valueText = null;
        
        public void SetValue(long value)
        {
            valueText.text = value.ToString();
        }
        public void SetValue(double value)
        {
            valueText.text = value.ToString();
        }
        public void SetValue(double value, Color color)
        {
            valueText.color = color;
            valueText.text = value.ToString();
        }
    }
}