using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.Serialization;

namespace Pjfb.Training
{
    public class TrainingCardRedoingConfirmModal : ModalWindow
    {
        private static readonly string DescriptionPath = "training.training_card_redoing_confirm.description";
        
        [SerializeField]
        private TextMeshProUGUI descriptionText = null;
        [SerializeField]
        private TextMeshProUGUI beforePointText = null;
        [SerializeField]
        private TextMeshProUGUI afterPointText = null;
        [SerializeField]
        private UIToggle showRedrawConfirmToggle = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            
            TrainingPointStatus pointStatus = (TrainingPointStatus)args;
            // 説明文
            descriptionText.text = string.Format(StringValueAssetLoader.Instance[DescriptionPath], pointStatus.handResetCostValue.GetStringNumberWithComma());
            
            // カード引き直し前のポイント
            beforePointText.text = pointStatus.value.GetStringNumberWithComma();
            
            long afterPoint = pointStatus.value - pointStatus.handResetCostValue;
            // カード引き直し後のポイント
            afterPointText.text = afterPoint.GetStringNumberWithComma();
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            // 表示と保存の値が逆
            TrainingUtility.IsConfirmPracticeCardRedrawModal = showRedrawConfirmToggle.isOn == false;
        }
        
        public void OnClickRedoing()
        {
            SetCloseParameter(true);
            Close();
        }
    }
}