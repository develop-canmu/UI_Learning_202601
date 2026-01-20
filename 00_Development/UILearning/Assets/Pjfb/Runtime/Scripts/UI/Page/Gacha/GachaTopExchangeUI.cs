using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopExchangeUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _point = null;
        
        long _storeId = 0;
        long _pointId = 0;

        /// <summary>データセット</summary>
        public void SetData(long storeId, long pointId){
            _storeId = storeId;
            _pointId = pointId;
            UpdatePoint(pointId);
        }

        public void OnClickExchangeButton() { 
            Pjfb.Shop.ShopExchangeModal.Open(_storeId, ()=>{
                UpdatePoint(_pointId);
            });
        }
        void UpdatePoint(long pointId) { 
            var point = GachaUtility.FetchPointValue(pointId);
            _point.text = point.ToString();
        }
    }
}
