using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb
{
    [RequireComponent(typeof(UIButton))]
    public class IntervalButton : MonoBehaviour
    {
        [SerializeField] [StringValue]
        private string stringId = string.Empty;
        
        [SerializeField]
        private TextMeshProUGUI buttonText = null;
        
        private UIButton _button = null;
        /// <summary>ボタン</summary>
        private UIButton button 
        {
            get
            {
                if(_button == null)
                {
                    _button = GetComponent<UIButton>();
                }
                return _button;
            }
        }

        private void Awake()
        {
            button.OnUpdateClickInterval += OnUpdate;
            button.OnCompleteClickInterval += OnComplete;
        }

        private void OnDestroy()
        {
            button.OnUpdateClickInterval -= OnUpdate;
            button.OnCompleteClickInterval -= OnComplete;
        }

        private void OnUpdate(float time)
        {
            buttonText.text = Mathf.CeilToInt(time).ToString();
        }
        
        private void OnComplete()
        {
            buttonText.text = StringValueAssetLoader.Instance[stringId];
        }
    }
}