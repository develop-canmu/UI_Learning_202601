using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Gacha;
using Logger = CruFramework.Logger;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using System;
using Pjfb.Home;

namespace Pjfb
{
    public class TutorialHomeTopPage : HomeTopPage
    {

        private TutorialSettings.Step tutorialStep = TutorialSettings.Step.None;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var response = AppManager.Instance.TutorialManager.GetHomeGetDataContainer().getData;
            var param = (TutorialSettings.Detail)args;
            tutorialStep = param.stepId;
            await base.OnPreOpen(new PageParam{homeApiResponse = response, isOnMessageShowPopups = true}, token);
        }

        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickNext).Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        public void OnClickNext()
        {
            switch (tutorialStep)
            {
                case TutorialSettings.Step.TrainingDeck:
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialTrainingPreparation, true, null);
                    break;
                case TutorialSettings.Step.Strengthen:
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialCharacter, true, null);
                    break;
                case TutorialSettings.Step.RivalryBattleMatching:
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialRivalry, true, null);
                    break;
            }
        }

        protected override void ShowHomePopups()
        {
            // チュートリアル中はホーム本来のポップアップ演出を避ける
        }
        
        private void Update()
        {
            // HomeTopPage.Updateでバックキー判定行ってるんでチュートリアルでは回避
        }
    }
}