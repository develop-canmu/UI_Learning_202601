using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UI;


using CruFramework.UI;

namespace Pjfb.Shop
{

    public class ShopTabSheetBeginner : ShopCategorySheet
    {
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.NoUse, bool isBan = false)
        {
            CruFramework.Logger.Log("TODO 初心者" + billingRewardBonusList.Count);
            isBanUser = isBan;
            // バナーをタップして開いた時のバッジ更新アクションを設定
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