using System;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingTargetPageView : MonoBehaviour
    {
        /// <summary>アニメーターのステート</summary>
        private static readonly string OpenState = "Open";
        /// <summary>アニメーターのステート</summary>
        private static readonly string CloseState = "Close";

        [SerializeField]
        private TMP_Text turnText = null;
        [SerializeField]
        private TrainingTargetView targetView = null;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private Canvas tapButtonCanvas = null;
        
        private Action onClosed = null;
        
        private bool isClosed = false;
        
        public void Open(TrainingGoal target, bool isSkip, Action onClosed)
        {
            OpenAsync(target, isSkip, onClosed).Forget();
        }
        
        private async UniTask OpenAsync(TrainingGoal target, bool isSkip, Action onClosed)
        {
            isClosed = false;
            this.onClosed = onClosed;
            // 残りターン表示
            turnText.text = target.restTurnCount.ToString();
            // 目標表示
            targetView.SetTarget(target);
            // アクティブ
            gameObject.SetActive(true);
            
            // タップボタンの表示切り替え
            tapButtonCanvas.enabled = isSkip == false;
            
            // 開くアニメーション
            await AnimatorUtility.WaitStateAsync(animator, OpenState);
            
            if(isSkip)
            {
                Close();
            }
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoCloseAsync().Forget();
#endif   
        }
        
        
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private async UniTask DebugAutoCloseAsync()
        {
            if (!TrainingChoiceDebugMenu.EnabledAutoChoiceTarget) return;

            await UniTask.DelayFrame(1);
            Close();
        }
#endif

        public void Close()
        {
            CloseAsync().Forget();
        }
        
        public async UniTask CloseAsync()
        {
            // 既に閉じている
            if(isClosed)return;
            isClosed = true;
            await AnimatorUtility.WaitStateAsync(animator, CloseState);
            gameObject.SetActive(false);
            onClosed();
        }
    }
}
