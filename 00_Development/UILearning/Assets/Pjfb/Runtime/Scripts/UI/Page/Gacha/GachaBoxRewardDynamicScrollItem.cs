using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxRewardDynamicScrollItem : ScrollDynamicItem
    {
        [SerializeField]
        GachaBoxRewardItem _prizeJsonView = null;
        
        protected override void OnSetView(object value){
            var param = (GachaBoxHighlightScrollDynamicItemData)value;
            _prizeJsonView.UpdateView(param.scrollItemParam);
        }
    }
}
