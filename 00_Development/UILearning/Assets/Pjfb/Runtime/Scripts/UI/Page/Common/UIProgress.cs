using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class UIProgress : MonoBehaviour
    {
        public class ProgressParams
        {
            public float maxValue;
            public float currentValue;
        }
        
        [SerializeField]
        private RectTransform progressBar = null;
        
        [SerializeField]
        private TextMeshProUGUI progressText = null;
        
        public void Show()
        {
            SetProgress(0);
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>進捗セット</summary>
        public void SetProgress(float value)
        {
            value = Mathf.Clamp01(value);
            // 進捗テキスト更新
            progressText.text = Mathf.FloorToInt(value * 100).ToString();
            // 進捗バー更新
            Vector2 anchorMax = progressBar.anchorMax;
            anchorMax.x = value;
            progressBar.anchorMax = anchorMax;
        }

        public void SetProgress(ProgressParams progressParams)
        {
            SetProgress(min: 0, max: progressParams.maxValue, value: progressParams.currentValue);
        }
        
        /// <summary>進捗セット</summary>
        public void SetProgress(float min, float max, float value)
        {
            float progress = Mathf.InverseLerp(min, max, value);
            SetProgress(progress);
        }
        
        /// <summary>進捗セット</summary>
        public void SetProgress(BigValue min, BigValue max, BigValue value)
        {
            BigValue clampedValue = BigValue.Clamp(value, min, max);
            float ratio = (float)BigValue.RatioCalculation(clampedValue, max);
            SetProgress(ratio);
        }
        
    }
}
