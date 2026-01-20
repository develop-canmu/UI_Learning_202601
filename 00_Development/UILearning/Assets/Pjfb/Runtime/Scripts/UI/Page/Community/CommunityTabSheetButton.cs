using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Community
{
    public class CommunityTabSheetButton : SheetSwitchButton<CommunitySheetManager, CommunityTabSheetType>
    {
        public Action OnTabClick;
        protected override void OnOpened()
        {
            OnTabClick?.Invoke();
            base.OnOpened();
        }
    }
}


