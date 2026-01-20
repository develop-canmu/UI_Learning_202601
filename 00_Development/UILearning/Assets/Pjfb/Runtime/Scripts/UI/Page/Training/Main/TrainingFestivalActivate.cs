
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.Training
{
    
    
    public class TrainingFestivalActivate : MonoBehaviour
    {
        
        private static readonly string OpenAnimation = "Open";
        private static readonly string CloseAnimation = "Close";
        
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private TMP_Text messageText = null;
        
        private Action onClosed = null;
        
        public void Open(TrainingMainArguments arguments, Action onClosed)
        {
            this.onClosed = onClosed;
            animator.SetTrigger(OpenAnimation);
            
            //　効果値
            long value = arguments.FestivalEffectStatus.value / 100;
            // 時間
            TimeSpan endAt = AppTime.Parse(arguments.FestivalEffectStatus.expireAt) - AppTime.Now;
            int limit = Mathf.Max(0, (int)endAt.TotalMinutes);
            
            // メッセージ
            messageText.text = string.Format(StringValueAssetLoader.Instance["event.training.bonus.activation.msg"], limit, value);
        }
        
        /// <summary>閉じる</summary>
        public void OnCloseButton()
        {
            CloseAsync().Forget();
        }
        
        public async UniTask CloseAsync()
        {
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            onClosed();
        }
    }
}