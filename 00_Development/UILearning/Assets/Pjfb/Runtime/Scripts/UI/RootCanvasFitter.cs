using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    // [ExecuteAlways]
    public class RootCanvasFitter : MonoBehaviour
    {
        [SerializeField]
        private bool fitWidth = false;
        
        [SerializeField]
        private int maxWidth = -1;
        
        [SerializeField]
        private bool fitHeight = false;
        
        [SerializeField]
        private int maxHeight = -1;
        
        [SerializeField]
        private bool delayOneFrame = false;
        
        private async void Start()
        {
            if(delayOneFrame)
            {
                await UniTask.NextFrame(this.GetCancellationTokenOnDestroy());
            }
            
#if UNITY_EDITOR
            if(AppManager.Instance == null)
	        {
                SetPositionAndSize(transform.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>());
                return;
            }
#endif

            SetPositionAndSize(AppManager.Instance.UIManager.RootCanvas.GetComponent<RectTransform>());
        }
        
        private void SetPositionAndSize(RectTransform targetTransform)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            // sizeDelta使うため
            Vector2 harf = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = harf; 
            rectTransform.anchorMin = harf;
            rectTransform.pivot = harf;

            // ポジション
            rectTransform.position = targetTransform.transform.position;
            
            // サイズ
            Vector2 size = rectTransform.sizeDelta;
            // 横のサイズを合わせる
            if(fitWidth)
            {
                size.x = targetTransform.sizeDelta.x;
                if(maxWidth > 0 && size.x > maxWidth)
                {
                    size.x = maxWidth;
                }
            }
            // 縦のサイズを合わせる
            if(fitHeight)
            {
                size.y = targetTransform.sizeDelta.y;
                if(maxHeight > 0 && size.y > maxHeight)
                {
                    size.y = maxHeight;
                }
            }
            rectTransform.sizeDelta = size;
        }
    }
}