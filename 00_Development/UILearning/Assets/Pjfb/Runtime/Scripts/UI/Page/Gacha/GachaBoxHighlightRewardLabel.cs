using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxHighlightRewardLabel : ScrollDynamicItem
    {
        public class Param {
            public bool isCurrent = false;
            public long lapsMin = 0;
            public long lapsMax = 0;
        }


        [SerializeField]
        GameObject _currentLabel = null;
        [SerializeField]
        TextMeshProUGUI _currentLabelText = null;
        
        [SerializeField]
        GameObject _defaultLabel = null;
        [SerializeField]
        TextMeshProUGUI _defaultLabelText = null;
        
        protected override void OnSetView(object value){
            var param = (GachaBoxHighlightScrollDynamicItemData)value;
            var labelParam = param.labelParam;
            string text = "";
            // 何週目の報酬かのラベルテキスト取得
            if( labelParam.lapsMin == labelParam.lapsMax ) {
                text = string.Format(StringValueAssetLoader.Instance["gacha.box_laps_label"], labelParam.lapsMin);
            } else if( labelParam.lapsMax >= GachaUtility.BoxGachaLoopLap ) {
                text = string.Format(StringValueAssetLoader.Instance["gacha.box_laps_label_max"], labelParam.lapsMin);
            } else {
                text = string.Format(StringValueAssetLoader.Instance["gacha.box_laps_label_area"], labelParam.lapsMin, labelParam.lapsMax);
            }

            _currentLabel.SetActive( labelParam.isCurrent );
            _defaultLabel.SetActive( !labelParam.isCurrent );
            _currentLabelText.text = text;
            _defaultLabelText.text = text;
        }
    }
}
