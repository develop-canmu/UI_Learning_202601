using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Common;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UI;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb.Shop
{
    public class ShopTabSheetPass : ShopCategorySheet
    {
        [SerializeField] private ListContainer listContainer;
        [SerializeField] private GameObject externalItemRoot;
        [SerializeField] private PossessionItemUi externalItemUi;
        [SerializeField] private RectTransform adjuster;
        private ShopManager.PurchaseType purchaseType;
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.NoUse, bool isBan = false)
        {
            CruFramework.Logger.Log("TODO パスタブ" + billingRewardBonusList.Count);
            data = billingRewardBonusList;
            isBanUser = isBan;
            OnUpdateTabBadge?.Invoke();
            purchaseType = LocalSaveManager.saveData.shopPurchaseType;
            // 自動レイアウト+ScrollGridを使用するため強制的に計算をする
            LayoutRebuilder.ForceRebuildLayoutImmediate(adjuster);
            UpdatedBillingRewardBonusList();
            ShopManager.SaveConfirmedBillingRewardId(billingRewardBonusList);
        }

        private void UpdatedBillingRewardBonusList()
        {
            var items = new List<ShopPassItem.ItemParams>();
            foreach (var reward in data)
            {
                if (purchaseType == ShopManager.PurchaseType.Point)
                {
                    var mBillingRewardAlternativePoint =
                        MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                            reward.mBillingRewardId);
                    if (mBillingRewardAlternativePoint == null) continue;
                }
                
                if (purchaseType == ShopManager.PurchaseType.External)
                {
                    BillingRewardMasterObject mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(reward.mBillingRewardId);
                    if (mBillingReward == null) continue;
                    BillingRewardAlternativePointMasterObject mBillingRewardAlternativePoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(mBillingReward.id, PointMasterObject.PointType.ExternalPoint);
                    if (mBillingRewardAlternativePoint == null) continue;
                    externalItemUi.SetCount(mBillingRewardAlternativePoint.mPointId, UserDataManager.Instance.GetExpiryPointValue(mBillingRewardAlternativePoint.mPointId), true);
                }
                
                var item = new ShopPassItem.ItemParams();
                item.bonus = reward;
                item.isNew = ShopManager.HasNewBillingReward(reward);
                item.isBan = isBanUser;
                item.PurchaseType = purchaseType;
                item.OnUpdateUi = () => OnUpdateUi?.Invoke();
                item.OnSelectedBillingRewardBonus = selectedBillingRewardBonusId => OnSelectedBillingRewardBonus?.Invoke(selectedBillingRewardBonusId);
                items.Add(item);
            }
            
            items = items.OrderByDescending(item =>
            {
                var mLonginPass = MasterManager.Instance.loginPassMaster.FindDataByMBillingRewardBonusId(item.bonus.mBillingRewardBonusId);
                var uTag = UserDataManager.Instance.tag?.FirstOrDefault(tempTag => tempTag == mLonginPass?.adminTagId);
                return uTag == null || uTag <= 0;
            }).ToList();
            listContainer.SetDataList(items);
            externalItemRoot.SetActive(purchaseType == ShopManager.PurchaseType.External);
        }
        
        public void OnClickPurchaseChange()
        {
            purchaseType = ShopManager.ChangePurchase(purchaseType,data);
            UpdatedBillingRewardBonusList();
        }
    }
}