using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Community
{
    public class FollowSheetButton : SheetSwitchButton<FollowSheetManager, FollowSheetType>
    {
        public Action OnTabClick;
        protected override void OnOpened()
        {
            OnTabClick?.Invoke();
            base.OnOpened();
        }
    }
}


