using System;
using System.Collections.Generic;
using CruFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pjfb
{
    public class UIDebugPanel : MonoBehaviour
    {
        enum Status
        {
            Good,
            Normal,
            Bad,
        }
        
        #region SerializeFields
        [SerializeField] private UISafeAreaTarget safeAreaTarget;
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI memoryText;
        [SerializeField] private TextMeshProUGUI currentTime;
        [SerializeField] private TextMeshProUGUI currentEnv;
        [Range(0, 5f)] [SerializeField] private float interval = 1.0f;
        #endregion

        #region PrivateFields
        private float timer = 0.0f;
        private Dictionary<Status, Color> statusColor = new Dictionary<Status, Color>();
        private Canvas rootCanvas;
        
        #endregion

        private void Awake()
        {
#if PJFB_REL
            Destroy(gameObject);
            return;
#endif
            statusColor.Add(Status.Good, Color.white);
            statusColor.Add(Status.Normal, Color.yellow);
            statusColor.Add(Status.Bad, Color.red);

            rootCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            // セーフエリア動的適用
            AppManager.Instance.UIManager.RootCanvas.GetComponent<UISafeArea>().AddTarget(safeAreaTarget);
        }

        #region PrivateMethods
        private void Update()
        {
            if (!rootCanvas.enabled) return;
            
            timer += Time.deltaTime;
            if (timer < interval) return;
            
            var fps = 1f / Time.deltaTime;
            var totalAllocatedMemory = (Profiler.GetTotalAllocatedMemoryLong() >> 10) / 1024;
            var totalReservedMemory = (Profiler.GetTotalReservedMemoryLong() >> 10) / 1024;

            var fpsStatus = GetFpsStatus(fps);
            var memoryStatus = GetAllocatedMemoryStatus(totalAllocatedMemory);
            
            fpsText.text = $"FPS:{fps:F1}";
            fpsText.color = statusColor[fpsStatus];
            memoryText.text = $"Mem:{totalAllocatedMemory}MB / {totalReservedMemory}MB";
            memoryText.color = statusColor[memoryStatus];
            // 現在時間
            currentTime.text = AppTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // 現在の環境
            currentEnv.text = AppEnvironment.CurrentEnvironment.ToString();
            // タイマーリセット
            timer -= interval;
        }

        /// <summary>
        /// FPSの状態を取得
        /// Good:30〜2520〜25
        /// Normal:25〜20
        /// Bad:20〜
        /// </summary>
        /// <param name="fps"></param>
        /// <returns></returns>
        Status GetFpsStatus(float fps)
        {
            var threshold = Application.targetFrameRate * 0.8f;
            if (fps >= threshold) return Status.Good;
            if (fps < threshold) return Status.Bad;
            return Status.Normal;
        }
        
        /// <summary>
        /// Memoryの状態を取得
        /// Good:〜500MB
        /// Normal:500MB〜
        /// Bad:1000MB〜
        /// </summary>
        /// <param name="fps"></param>
        /// <returns></returns>
        Status GetAllocatedMemoryStatus(long memory)
        {
            if (memory >= 500) return Status.Bad;
            if (memory < 250) return Status.Good;
            return Status.Normal;
        }

        public void Show()
        {
            rootCanvas.enabled = true;
        }

        public void Hide()
        {
            rootCanvas.enabled = false;
        }

        #endregion
    }
}
