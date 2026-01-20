using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class EventTopEventPointRewardButtonImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetEventTopEventPointRewardButtonImagePath(id);
        }
    }
}