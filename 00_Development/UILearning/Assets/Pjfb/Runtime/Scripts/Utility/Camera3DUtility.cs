using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class Camera3DUtility : MonoBehaviour
{
    public enum EventEnum
    {
        OnRTChanged,
        OnEnableRTBG,
    }
    public class CameraEvent : UnityEvent<EventEnum, object>{}
    private static Camera3DUtility _inst; 
    private Camera _attachedCamera;
    private RenderTexture _targetRT;

    [SerializeField] private GameObject training3DRoot;

    private int _oldH;
    private int _oldW;
    public float Scale { get; private set; }
    public CameraEvent cameraEvent;
    private RectTransform rootCanvasRect;

    public Vector3 RootCanvasPosition => rootCanvasRect.position;

    private void Awake()
    {
        if (_inst != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        cameraEvent = new CameraEvent();
        _inst = this;
        _attachedCamera = GetComponent<Camera>();
    }

    public void ResizeRT(float scale)
    {
        var refSize = rootCanvasRect.sizeDelta;
        Scale = scale;
        var targetSize = new Vector2(Mathf.Round(refSize.x * Scale / 2.0f) * 2.0f,
            Mathf.Round(refSize.y * Scale / 2.0f) * 2.0f);
        _oldW = Mathf.CeilToInt(targetSize.x);
        _oldH = Mathf.CeilToInt(targetSize.y);
        _targetRT = new RenderTexture(_oldW, _oldH, 24);
        _targetRT.useDynamicScale = true;
        _attachedCamera.targetTexture = _targetRT;
        cameraEvent.Invoke(EventEnum.OnRTChanged, _targetRT);
    }

    private void CreateRT()
    {
        var size = rootCanvasRect.sizeDelta;
        var ratiox = 1080/ size.x;
        var ratioy = 1920/ size.y;
        ResizeRT(Mathf.Min(ratiox, ratioy));
    }

    public static Camera3DUtility Instance => _inst;

    public RenderTexture GetRenderTarget => _targetRT;

// #if UNITY_EDITOR
//     private void Update()
//     {
//         if (_oldH != Screen.height || _oldW != Screen.width)
//         {
//             Destroy(_targetRT);
//             CreateRT();
//         }
//     }
// #endif

    public void ShowRenderTextureBackGround(bool isShow)
    {
        cameraEvent.Invoke(EventEnum.OnEnableRTBG, isShow);
    }
    public void InitRT()
    {
        cameraEvent = new CameraEvent();
        rootCanvasRect = AppManager.Instance.UIManager.RootCanvas.GetComponent<RectTransform>();
        CreateRT();
    }
    public void SetEnable3DTraningRoot(bool value)
    {
        training3DRoot.SetActive(value);
    }

    private void OnDestroy()
    {
        _inst = null;
        _targetRT.Release();
    }
}
