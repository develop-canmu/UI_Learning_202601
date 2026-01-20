using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System.Linq;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingInspirationIconCrossfade : TrainingInspirationCrossfadeBase<TrainingInspirationIcon>
    {
        protected override void OnSetView(TrainingInspirationIcon view, long id)
        {
            view.SetInspirationId( id );
        }
    }
}