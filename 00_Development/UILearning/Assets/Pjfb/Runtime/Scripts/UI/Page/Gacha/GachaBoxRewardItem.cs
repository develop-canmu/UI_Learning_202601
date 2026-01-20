using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxRewardItem : MonoBehaviour
    {
        public class Param{
            public PrizeJsonViewData prizeJsonData = null;
            public long quantity = 0;
            public long totalaquantity = 0;
            public bool isHighlight = false;
            public bool forceCover = false;
            public bool isHighlightReword = false;
        }

        [SerializeField]
        PrizeJsonView _prizeJsonView = null;
        [SerializeField]
        TextMeshProUGUI _nameText = null;
        [SerializeField]
        TextMeshProUGUI _quantityTitleText = null;
        [SerializeField]
        TextMeshProUGUI _quantityText = null;
        
        [SerializeField]
        GameObject[] _coverObjects = null;

        [SerializeField]
        GameObject _highlightFrame = null;
        [SerializeField]
        GameObject _highlightBadge = null;


        public void UpdateView(Param param) {
            _prizeJsonView.SetView( param.prizeJsonData );
            _nameText.text = param.prizeJsonData.Name;

            if (param.totalaquantity > 0)
            {
                _quantityText.text = String.Format("{0:#,0}/{1:#,0}", param.quantity, param.totalaquantity);
            }
            else
            {
                _quantityText.text = String.Format("{0:#,0}", param.quantity);
            }

            foreach( var cover in _coverObjects ){
                cover.SetActive( param.forceCover || param.quantity <= 0 );
            }
            _highlightFrame.SetActive( param.isHighlight );
            _highlightBadge.SetActive( param.isHighlight );

            var stringKey = param.isHighlightReword ? "gacha.box_lot_limit" : "gacha.box_content_quantity";
            _quantityTitleText.text = StringValueAssetLoader.Instance[stringKey];
        }
    }
}
