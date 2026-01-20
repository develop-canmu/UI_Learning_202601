using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Pjfb
{
    public class ModalSizeValueSetter : MonoBehaviour
    {
        [SerializeField]
        [ModalSizeValue]
        private string modalSizeId = string.Empty;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateSize();
        }
#endif
        
        private void UpdateSize()
        {
            if(ModalSizeValueAssetLoader.Instance.HasKey(modalSizeId))
            {
                RectTransform rectTransform = (RectTransform)transform;
                rectTransform.sizeDelta = ModalSizeValueAssetLoader.Instance[modalSizeId];
            }
        }
        
        private void Start()
        {
            UpdateSize();
        }
    }
}