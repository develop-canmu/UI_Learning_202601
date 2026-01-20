using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingOptionModal : ModalWindow
    {
        [SerializeField]
        private UIToggle confirmRestToggle = null;
        
        [SerializeField]
        private UIToggle confirmPracticeGameToggle = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 休憩確認
            confirmRestToggle.isOn = TrainingUtility.IsConfirmRestModal;
            // 練習試合確認
            confirmPracticeGameToggle.isOn = TrainingUtility.IsConfirmPracticeGameModal;
            
            return base.OnPreOpen(args, token);
        }
        protected override void OnClosed()
        {
            // 休憩確認
            TrainingUtility.IsConfirmRestModal = confirmRestToggle.isOn;
            // 練習試合確認
            TrainingUtility.IsConfirmPracticeGameModal = confirmPracticeGameToggle.isOn;
        }
    }
}