using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxRewardScrollItem : ScrollGridItem
    {
        

        [SerializeField]
        GachaBoxRewardItem _prizeJsonView = null;
        
        protected override void OnSetView(object value){
            var param = (GachaBoxRewardItem.Param)value;
            _prizeJsonView.UpdateView(param);
        }
    }
}
