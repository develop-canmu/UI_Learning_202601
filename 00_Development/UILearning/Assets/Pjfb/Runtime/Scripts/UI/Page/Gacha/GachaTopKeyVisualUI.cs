using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopKeyVisualUI : MonoBehaviour
    {
        [SerializeField]
        private GachaTopLargeBannerWebTexture largeBannerTexture = null;
        
        [SerializeField]
        private GameObject endAtRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI endAtText = null;

        /// <summary>データセット</summary>
        public void SetData(GachaSettingData data)
        {
            // 画像セット
            largeBannerTexture.SetTexture(data.DesignNumber);
            
            // 無期限ガチャ
            if(data.IsIndefinitePeriod)
            {
                endAtRoot.SetActive(false);
            }
            else
            {
                // 期間を表示
                endAtRoot.SetActive(true);
                endAtText.text = StringUtility.ToEndAtString( data.EndAt ); 
            }
        }
    }
}
