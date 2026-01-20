using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Shop
{
    public class ShopPackIconGridItem : ScrollGridItem
    {
        public class Data
        {
            public readonly Master.PrizeJsonWrap MasterPrizeJsonWrap;
            public readonly Networking.App.Request.PrizeJsonWrap RequestPrizeJsonWrap;
            public readonly bool IsPaid;

            public Data(Master.PrizeJsonWrap _wrap, bool isPaid)
            {
                MasterPrizeJsonWrap = _wrap;
                RequestPrizeJsonWrap = null;
                IsPaid = isPaid;
            }

            public Data(Networking.App.Request.PrizeJsonWrap _wrap)
            {
                RequestPrizeJsonWrap = _wrap;
                MasterPrizeJsonWrap = null;
                IsPaid = false;
            }
        }
        
        [SerializeField] private ShopPackIcon shopPackIcon;
        
        private Data data;

        protected override void OnSetView(object value)
        {
            data = (Data) value;
            if (data.MasterPrizeJsonWrap != null)
            {
                shopPackIcon.SetView(data.MasterPrizeJsonWrap, data.IsPaid);
            }
            else
            {
                shopPackIcon.SetView(data.RequestPrizeJsonWrap);
            }
        }
    }
}