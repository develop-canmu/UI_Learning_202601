using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingHomeButton : AutoTrainingStateUpdateView
    {
        protected override string AnyAutoTrainingStatusText()
        {
            string remainingTime = FinishTime.GetRemainingString(AppTime.Now);
            return string.Format(StringValueAssetLoader.Instance["auto_training.remainingTime"], remainingTime);
        }

        protected override string CompleteAutoTrainingStatusText()
        {
            return StringValueAssetLoader.Instance["auto_training.complete"];
        }

        protected override string AllEmptyAutoTrainingStatusText()
        {
            return StringValueAssetLoader.Instance["auto_training.possible"];
        }
    }
}