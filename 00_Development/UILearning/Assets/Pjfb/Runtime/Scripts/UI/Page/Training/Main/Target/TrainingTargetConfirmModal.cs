using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb.Training
{
    public class TrainingTargetConfirmModal : ModalWindow
    {
        [SerializeField]
        private TMPro.TMP_Text progressText = null;
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingMainArguments mainArguments = (TrainingMainArguments)args;
            
            // スクロールアイテム
            scrollGrid.SetItems(mainArguments.Pending.goalList);
            // 進捗テキスト更新
            progressText.text = string.Format(StringValueAssetLoader.Instance["training.target.progress"], mainArguments.GetCompletedTargetCount(), mainArguments.Pending.goalList.Length);
            
            return base.OnPreOpen(args, token);
        }
    }
}