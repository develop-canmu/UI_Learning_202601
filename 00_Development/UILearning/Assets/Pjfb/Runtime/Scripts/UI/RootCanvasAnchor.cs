using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    [ExecuteAlways]
    public class RootCanvasAnchor : MonoBehaviour
    {
        [SerializeField]
        private bool isHorizontal = false;        
        [SerializeField]
        private bool isVertical = false;
        
        [SerializeField]
        private Vector3 offset = Vector2.zero;
        
        private void Start()
        {
#if UNITY_EDITOR
            if(AppManager.Instance == null)
	        {
                SetAnchor(transform.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>());
                return;
            }
#endif

            SetAnchor(AppManager.Instance.UIManager.RootCanvas.GetComponent<RectTransform>());
        }

        private void SetAnchor(RectTransform targetTransform)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            Vector3 position = targetTransform.position;
            if(isHorizontal == false)
            {
                position.x = rectTransform.position.x;
            }
            
            if(isVertical == false)
            {
                position.y = rectTransform.position.y;
            }
            
            // ポジション
            rectTransform.position = position;
            
            // サイズ
            Vector2 size = targetTransform.rect.size;
            Vector2 pivot = rectTransform.pivot;
            Vector3 p = rectTransform.localPosition;
            // 横のサイズを合わせる
            if(isHorizontal)
            {
                p.x += (pivot.x - 0.5f) * size.x;
            }

            // 縦のサイズを合わせる
            if(isVertical)
            {
                p.y += (pivot.y - 0.5f) * size.y;
            }
            
            rectTransform.localPosition = p + offset;
            
        }
    }
}