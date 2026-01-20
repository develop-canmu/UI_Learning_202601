using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using CruFramework.UI;
using Pjfb.Master;


namespace Pjfb.Shop
{

    public class ShopTabSheetLimited : ShopCategorySheet
    {
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.NoUse, bool isBan = false)
        {
            CruFramework.Logger.Log("TODO 期間限定" + billingRewardBonusList.Count);
            isBanUser = isBan;
            var bannerList = CreateImageBannerIds(() =>
            {
                scrollGrid.RefreshItemView();
                OnUpdateTabBadge?.Invoke();
            });
            OnUpdateTabBadge?.Invoke();
            scrollGrid.SetItems(bannerList);
        }
     
    }
    
    
}