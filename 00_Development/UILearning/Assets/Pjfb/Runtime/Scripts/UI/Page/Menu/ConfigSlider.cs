using System;
using CruFramework.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class ConfigSlider : MonoBehaviour
    {
        #region Params
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private UIButton minusButton;
        [SerializeField] private UIButton plusButton;
        [SerializeField] private Toggle muteToggle;

        public Action OnEditValue;
        
        private float lastValue = 0;
        
        #endregion

        #region Life Cycle

        public void Init(int value,int last)
        {
            lastValue = last;
            SetSliderValue(value);
        }

        #endregion


        #region EventListerers
        
        public void OnSliderValueChanged(float sliderValue)
        {
            SetSliderValue(sliderValue);
            OnEditValue?.Invoke();
            SEManager.PlaySE(SE.se_common_click);
        }
        
        public void OnClickPlus()
        {
            int currentValue = GetSliderValue();
            SetSliderValue(slider.value + 1);
            if(currentValue != GetSliderValue()) OnEditValue?.Invoke();
        }

        public void OnClickMinus()
        {
            int currentValue = GetSliderValue();
            var value = slider.value - 1;
            SetSliderValue(value);
            if(currentValue != GetSliderValue()) OnEditValue?.Invoke();
        }
        
        public void OnClickMute(bool value)
        {
            if (value)
            {
                SetSliderValue(lastValue);
            }
            else
            {
                lastValue = Mathf.FloorToInt(Mathf.Clamp(slider.value, slider.minValue, slider.maxValue));
                SetSliderValue(0);
            }
            OnEditValue?.Invoke();
        }

        #endregion

        #region Other
        public float SetSliderValue(float value)
        {
            int intValue = Mathf.FloorToInt(Mathf.Clamp(value, slider.minValue, slider.maxValue));
            slider.SetValueWithoutNotify(intValue);
            valueText.text = intValue.ToString("N0");
            muteToggle.SetIsOnWithoutNotify(slider.value > 0);
            return slider.value;
        }

        public int GetSliderValue()
        {
            return  Mathf.FloorToInt(slider.value);;
        }
        
        public int GetLastSliderValue()
        {
            return  Mathf.FloorToInt(lastValue);
        }
        #endregion
        
    }
}