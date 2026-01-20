using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.UI
{
    // [ExecuteAlways]
    [RequireComponent(typeof(CanvasScaler))]
    [FrameworkDocument("UI", nameof(UISafeArea), "セーフエリア設定用コンポーネント")]
    public class UISafeArea : MonoBehaviour
    {
        [SerializeField]
        private UIResolutionSettings resolutionSettings = null;
        
        [SerializeField]
        private bool fixedResolutionX = false;
        
        [SerializeField]
        private bool fixedResolutionY = false;

        [SerializeField]
        private List<UISafeAreaTarget> targets = null;

        private CanvasScaler canvasScaler = null;

        // 適用されたか判定するために使用する
        private Rect safeArea = new Rect(0, 0, 1, 1);
        
        /// <summary>ターゲット追加</summary>
        public void AddTarget(UISafeAreaTarget target)
        {
            targets.Add(target);
            UpdateSafeArea();
        }

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        private void UpdateSafeArea()
        {
            // 画面サイズに合わせてUIも拡大する
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            // キャンバスのサイズが基準の解像度よりも大きくなるようにする
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            // 基準の解像度がセーフエリア内に収まるようにCanvasScalerの解像度を調整
            Vector2 resolution = resolutionSettings.ReferenceResolution;
            resolution.x = (int)(resolution.x * (Screen.width / (float)Screen.safeArea.width));
            resolution.y = (int)(resolution.y * (Screen.height / (float)Screen.safeArea.height));
            canvasScaler.referenceResolution = resolution;

            // セーフエリア計算
            // Min
            Vector2 safeAreaMin = Screen.safeArea.min;
            safeAreaMin.x /= Screen.width;
            safeAreaMin.y /= Screen.height;
            // Max
            Vector2 safeAreaMax = Screen.safeArea.max;
            safeAreaMax.x /= Screen.width;
            safeAreaMax.y /= Screen.height;

            // ターゲットに適用
            foreach (UISafeAreaTarget target in targets)
            {
                // セーフエリア適用
                target.SafeArea.anchoredPosition = Vector2.zero;
                target.SafeArea.sizeDelta = Vector2.zero;
                target.SafeArea.anchorMin = safeAreaMin;
                target.SafeArea.anchorMax = safeAreaMax;
                target.SafeArea.pivot = new Vector2(0.5f, 0.5f);
                
                // 固定エリアの指定がない
                if(target.FixedArea == null) 
                {
                    continue;
                }
                
                // 解像度を固定する場合の処理
                Vector2 fixedMin = Vector2.zero;
                Vector2 fixedMax = Vector2.one;
                Vector2 fixedSize = Vector2.zero;
                
                // 横幅が固定
                if(fixedResolutionX)
                {
                    fixedMin.x = 0.5f;
                    fixedMax.x = 0.5f;
                    fixedSize.x = resolutionSettings.ReferenceResolution.x;
                }
                // 横幅が可変  
                else
                {
                    // 最大サイズを超えた
                    if(resolutionSettings.MaxResolution.x > 0 && target.SafeArea.rect.width > resolutionSettings.MaxResolution.x)
                    {
                        fixedMin.x = 0.5f;
                        fixedMax.x = 0.5f;
                        fixedSize.x = resolutionSettings.MaxResolution.x;
                    }
                }
                
                // 縦幅が固定
                if(fixedResolutionY)
                {
                    fixedMin.y = 0.5f;
                    fixedMax.y = 0.5f;
                    fixedSize.y = resolutionSettings.ReferenceResolution.y;
                }
                // 縦幅が可変
                else
                {
                    // 最大サイズを超えた
                    if(resolutionSettings.MaxResolution.y > 0 && target.SafeArea.rect.height > resolutionSettings.MaxResolution.y)
                    {
                        fixedMin.y = 0.5f;
                        fixedMax.y = 0.5f;
                        fixedSize.y = resolutionSettings.MaxResolution.y;
                    }
                }
                
                // 固定エリア適用
                target.FixedArea.anchoredPosition = Vector2.zero;
                target.FixedArea.sizeDelta = fixedSize;
                target.FixedArea.anchorMin = fixedMin;
                target.FixedArea.anchorMax = fixedMax;
                target.FixedArea.pivot = new Vector2(0.5f, 0.5f);
            }
            
            safeArea = Screen.safeArea;
        }

        private void Update()
        {
            if (resolutionSettings == null) return;

            // 一部のAndroid端末でStart()時でもScreen.safeAreaが正しい値で取得できない可能性があるのでチェック
            if (safeArea != Screen.safeArea)
            {
                UpdateSafeArea();
            }
        }
    }
}
