using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{

    public class UiSlider : MonoBehaviour
    {
        [SerializeField] private UIButton addButton;
        [SerializeField] private UIButton subButton;
        [SerializeField] private Slider slider;

        public bool IsWholeNumber
        {
            get { return slider.wholeNumbers; }
            set { slider.wholeNumbers = value; }
        }


        public void SetSliderRange(long min, long max)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }

        public float SetSliderValue(float value)
        {
            slider.value = value;
            return slider.value;
        }

        public float GetSliderValue()
        {
            return slider.value;
        }

        public void SetSliderInteractable(bool interactable)
        {
            slider.interactable = interactable;
        }
        // Add event to OnValueChange if needed
        public void SetButtonInteractable()
        {
            addButton.interactable = GetSliderValue() < slider.maxValue;
            subButton.interactable = GetSliderValue() > slider.minValue;
        }

        public void OnClickButton(int value)
        {
            SetSliderValue(GetSliderValue() + value);
        }
        
    }
}
