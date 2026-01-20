using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using TMPro;
using UnityEngine;

namespace Pjfb
{

    
    public class RankRewardScrollItem : ScrollGridItem
    {
        [SerializeField] private TextMeshProUGUI tempText;
        
        long tempId = 0;
        
        
        protected override void OnSetView(object value)
        {
            tempId = (long)value;
            tempText.text = tempId.ToString();
        }
    }
}