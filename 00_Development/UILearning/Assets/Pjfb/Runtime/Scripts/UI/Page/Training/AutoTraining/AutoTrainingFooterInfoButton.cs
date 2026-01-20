using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Extensions;

namespace Pjfb.Training
{
    public class AutoTrainingFooterInfoButton : AutoTrainingStateUpdateView
    {
        protected override string AnyAutoTrainingStatusText()
        {
            string remainingTime = FinishTime.GetRemainingString(AppTime.Now);
            return string.Format(StringValueAssetLoader.Instance["auto_training.switch_button.remaining_time"], remainingTime);

        }

        protected override string CompleteAutoTrainingStatusText()
        {
            return StringValueAssetLoader.Instance["auto_training.complete"];
        }

        protected override string AllEmptyAutoTrainingStatusText()
        {
            return StringValueAssetLoader.Instance["auto_training.can_training"];
        }
    }
}