using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    public class UILoadingProgress : MonoBehaviour
    {
        [SerializeField]
        private Slider progressBar = null;
        
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
            progressBar.value = value;
        }
        
        /// <summary>進捗セット</summary>
        public void SetProgress(float min, float max, float value)
        {
            float progress = Mathf.InverseLerp(min, max, value);
            SetProgress(progress);
        }
    }
}