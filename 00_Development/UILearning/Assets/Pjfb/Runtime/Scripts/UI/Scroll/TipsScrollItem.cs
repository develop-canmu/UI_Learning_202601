using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using TMPro;
using UnityEngine;


namespace Pjfb
{
    public class TipsScrollItem : ScrollGridItem
    {
        [SerializeField] private TextMeshProUGUI contentText;

        protected override void OnSetView(object value)
        {
            contentText.text = (string)value;
        }


        
        
    }
}
