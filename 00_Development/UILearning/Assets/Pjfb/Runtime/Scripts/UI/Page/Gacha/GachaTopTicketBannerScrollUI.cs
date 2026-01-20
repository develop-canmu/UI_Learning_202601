using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaTopTicketBannerScrollUI : MonoBehaviour
    {
        public ScrollGrid ScrollGrid { get { return scrollGrid; } }
        public bool Exist => viewData.Count > 0;

        [SerializeField]
        private TextMeshProUGUI noneText = null;

        [SerializeField]
        private ScrollGrid scrollGrid = null;

        [SerializeField]
        private GameObject endAtRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI endAtText = null;


        List<GachaTopTicketBannerScrollItem.Param> viewData = null;    
        GachaTopPageData data = null;   


        public void Init( GachaTopPageData data  ){
            this.data = data;
            var ticketDataList =  data.GetGachaDatas(GachaType.Ticket);
            viewData = new List<GachaTopTicketBannerScrollItem.Param>();
            if( ticketDataList != null ) {
                foreach( var ticketData in ticketDataList ){
                    if( !ticketData.IsEnable || !ticketData.HasPoint ) {
                        continue;
                    }
                    var param = new GachaTopTicketBannerScrollItem.Param();
                    param.data = ticketData;
                    param.page = data;
                    viewData.Add(param);
                }
            }

            ScrollGrid.SetItems(viewData);
            var exist = Exist;
            noneText.gameObject.SetActive( !exist );
            if( exist ) {
                GachaTopTicketBannerScrollItem currentItem = (GachaTopTicketBannerScrollItem)ScrollGrid.GetItem(data.CurrentTicketGachaData);
                currentItem.Select(true);
            }
        }

        public void UpdateTicketEndAtView()
        {
            if (!Exist)
            {
                endAtRoot.SetActive(false);
                return;
            }

            var currentTicketGachaData = data.CurrentTicketGachaData;
            // 無期限ガチャ
            if(currentTicketGachaData.IsIndefinitePeriod)
            {
                endAtRoot.SetActive(false);
            }
            else
            {
                // 期間を表示
                endAtRoot.SetActive(true);
                endAtText.text = StringUtility.ToEndAtString( currentTicketGachaData.EndAt ); 
            }
        }
    }
}