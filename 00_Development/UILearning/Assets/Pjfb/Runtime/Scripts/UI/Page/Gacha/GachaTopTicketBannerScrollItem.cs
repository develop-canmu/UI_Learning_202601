using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Gacha
{
    public class GachaTopTicketBannerScrollItem : ScrollGridItem
    {
        public class Param {
            public GachaSettingData data = null;
            public GachaTopPageData page = null;

            public override bool Equals(System.Object obj){
                if( obj is GachaSettingData ) {
                    return data == obj;        
                }
                return this == obj;
            }

            public override int GetHashCode(){
                return base.GetHashCode();
            }
        }
        [SerializeField]
        private GachaTopTicketBannerWebTexture ticketBannerTexture = null;
        
        [SerializeField]
        private TextMeshProUGUI ticketPossessionCountText = null;

        [SerializeField]
        private GameObject selectImage = null;
        
        private GachaSettingData gachaData = null;
        private GachaTopPageData page = null;

        protected override void OnSetView(object value)
        {
            var param = (Param)value;
            gachaData = param.data;
            page = param.page;

            // 画像セット
            ticketBannerTexture.SetTexture(gachaData.DesignNumber);
            // 所持数
            foreach (GachaCategoryData categoryData in gachaData.CategoryDatas)
            {
                if(categoryData != null)
                {
                    long possessionCount = 0;
                    if(UserDataManager.Instance.point.data.ContainsKey(categoryData.PointId))
                    {
                        possessionCount = UserDataManager.Instance.point.Find(categoryData.PointId).value;
                    }
                    // テキストセット
                    ticketPossessionCountText.text = possessionCount.ToString();
                    break;
                }
            }
            selectImage.SetActive(gachaData.GachaSettingId == page.CurrentGachaData.GachaSettingId);
        }
        
        public void Select(bool value)
        {
            selectImage.SetActive(value);
        }
        
        public void OnClick()
        {
            TriggerEvent(gachaData);
        }
    }
}
