using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;


namespace Pjfb.Training
{
    public class TrainingRestPage : TrainingPageBase
    {
        /// <summary>遷移までの時間</summary>
        private const float WaitDuration = 1.0f;
        
        // 待機時間
        private float waitTimer = 0;
        // 移動済み
        private bool isMovedPage = false;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 時間初期化
            waitTimer = 0;
            isMovedPage = false;
            
            TrainingEventMasterObject mTrainingEvent = MasterManager.Instance.trainingEventMaster.FindData(MainArguments.TrainingEvent.mTrainingEventId);
            // イベント名を休息に設定
            MainPageManager.Header.ShowEventView( mTrainingEvent.name);
            // キャラを表示
            MainPageManager.Character.gameObject.SetActive(true);
            
            // コンディション変化のメッセージ
            if(MainArguments.Reward != null && MainArguments.Reward.condition != 0)
            {
                SetMessage( TrainingUtility.GetConditionChangeMessage(MainArguments.Reward.condition) );
            }
            
            return base.OnPreOpen(args, token);
        }

        private void Update()
        {
            if(isMovedPage)return;
            
            waitTimer += Time.deltaTime;
            
            // 一定時間経過してメッセージが表示されてない
            if(waitTimer >= WaitDuration && IsShowMessage() == false)
            {
                OpenPage(TrainingMainPageType.Adv, MainArguments);
                isMovedPage = true;
            }
        }
    }
}
