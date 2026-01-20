using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Logger = CruFramework.Logger;

namespace Pjfb.Shop
{
    public class ShopTabSheetExchange : ShopCategorySheet
    {
        [SerializeField] private ListContainer listContainer;
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.NoUse, bool isBan = false)
        {
            var items = new Dictionary<long,ShopExchangeItemGroup.Data>();
            var categories = MasterManager.Instance.commonStoreCategoryMaster.GetAvailableCommonStoreCategory();
            foreach (var category in categories)
            {
                if (!items.ContainsKey(category.type))
                {
                    items[category.type] = new ShopExchangeItemGroup.Data();
                    items[category.type].storeList = new List<CommonStoreCategoryMasterObject>();
                    items[category.type].exchangeStoreType = (ExchangeStoreType)category.type;
                }
                items[category.type].storeList.Add(category);
            }
            OnUpdateTabBadge?.Invoke();
            var containerData = items.Values.OrderBy(item => item.exchangeStoreType).ToList();
            listContainer.SetDataList(containerData);
        }
    }
}