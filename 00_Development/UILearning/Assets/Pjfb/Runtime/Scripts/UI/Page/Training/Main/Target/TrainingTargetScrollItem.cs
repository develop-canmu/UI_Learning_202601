using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class TrainingTargetScrollItem : ScrollGridItem
    {
    
        [SerializeField]
        private TrainingTargetView targetView = null;

        protected override void OnSetView(object value)
        {
            TrainingGoal target = (TrainingGoal)value;
            targetView.SetTarget(target);
        }
    }
}