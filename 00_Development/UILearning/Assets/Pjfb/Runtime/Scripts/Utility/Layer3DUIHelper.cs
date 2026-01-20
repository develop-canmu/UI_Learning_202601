using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Layer3DUIHelper : MonoBehaviour
{
    [SerializeField] private RawImage attachedRawImage;
    [SerializeField] private RectTransform attachedImageRect;
    [SerializeField] private RectTransform bg;
    [SerializeField] private bool isConstraintToRootCanvas = false;
    
    async void Awake()
    {
        // Imageが貼られる前は表示を切っておく
        attachedRawImage.enabled = false;
        
        await UniTask.WaitUntil(() => Camera3DUtility.Instance != null);
        Camera3DUtility.Instance.cameraEvent.AddListener(OnCameraEvent);
        if (isConstraintToRootCanvas)
        {
            attachedRawImage.transform.position = Camera3DUtility.Instance.RootCanvasPosition;    
        }
        
        UpdateRT();
    }

    private void OnCameraEvent(Camera3DUtility.EventEnum camEvent, object value)
    {
        switch (camEvent)
        {
            case Camera3DUtility.EventEnum.OnRTChanged:
            {
                UpdateRT();
            }
                break;
            case Camera3DUtility.EventEnum.OnEnableRTBG:
            {
                var isEnable = (bool)value;
                bg.gameObject.SetActive(isEnable);
            }
                break;
        }
    }

    private void UpdateRT()
    {
        attachedRawImage.enabled = true;
        attachedRawImage.texture = Camera3DUtility.Instance.GetRenderTarget;
        attachedRawImage.SetNativeSize();
        attachedRawImage.transform.localScale = Vector2.one / Camera3DUtility.Instance.Scale;
        bg.sizeDelta = attachedImageRect.sizeDelta;
        bg.transform.localScale = attachedRawImage.transform.localScale;
    }

    private void OnDestroy()
    {
        if (Camera3DUtility.Instance == null) return;
        Camera3DUtility.Instance.cameraEvent.RemoveListener(OnCameraEvent);
    }
}
