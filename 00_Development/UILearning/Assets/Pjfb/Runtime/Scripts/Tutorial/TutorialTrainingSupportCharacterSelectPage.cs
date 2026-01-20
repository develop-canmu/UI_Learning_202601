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

namespace Pjfb
{
    public class TutorialTrainingSupportCharacterSelectPage : TrainingSupportCharacterSelectPage
    {
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }

        public new void OnSelected()
        {
            Arguments.SelectedSupportCharacterId = selectedCharacterId;
            OnSelectedAction().Forget();
        }

        private async UniTask OnSelectedAction()
        {
            TutorialTrainingPreparation m = (TutorialTrainingPreparation)Manager;
            await UniTask.NextFrame();
            m.PrevPage();
        }
        
    }
}