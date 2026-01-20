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
    public class TutorialTrainingCharacterSelectPage : TrainingCharacterSelectPage
    {
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }
        
        public new void OnNextButton()
        {
            TutorialTrainingPreparation m = (TutorialTrainingPreparation)Manager;
            m.OpenPage(TrainingPreparationPageType.SupportCharacterDeckSelect, true, Arguments);
        }
    }
}