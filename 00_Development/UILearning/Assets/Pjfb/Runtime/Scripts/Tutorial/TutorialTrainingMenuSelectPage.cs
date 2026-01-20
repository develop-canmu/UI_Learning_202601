using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Gacha;
using Logger = CruFramework.Logger;
using Pjfb.Networking.App.Request;
using System;
using Pjfb.Training;
using Unity.VisualScripting;

namespace Pjfb
{
    public class TutorialTrainingMenuSelectPage : TrainingMenuSelectPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            
            // 表示するId
            List<TrainingMenuItem.Arguments> menuArgs = new List<TrainingMenuItem.Arguments>();
            // リストに追加
            menuArgs.Add( new TrainingMenuItem.Arguments(AppManager.Instance.TutorialManager.GetTutorialTrainingScenarioId(), false) );
            scrollBanner.SetBannerDatas(menuArgs);
        }
        
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        public new void OnSelectedButton()
        {
            TutorialTrainingPreparation m = (TutorialTrainingPreparation)Manager;
            var arguments = new TrainingPreparationArgs();
            arguments.TrainingScenarioId = AppManager.Instance.TutorialManager.GetTutorialTrainingScenarioId();
            m.OpenPage(TrainingPreparationPageType.CharacterSelect, false, arguments);
        }
        
    }
}