using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using CruFramework.UI;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine.UI;


namespace Pjfb.Shop
{

    public class ShopTabSheetSecret : ShopCategorySheet
    {
        
        [SerializeField] private ScrollGrid scrollGrid = null;
        [SerializeField] private GameObject externalItemRoot;
        [SerializeField] private PossessionItemUi externalItemUi;
        [SerializeField] private RectTransform adjuster;
        private ShopManager.PurchaseType purchaseType;
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.NoUse, bool isBan = false)
        {
            CruFramework.Logger.Log("TODO シークレット" + billingRewardBonusList.Count);
            data = billingRewardBonusList;
            isBanUser = isBan;
            OnUpdateTabBadge?.Invoke();
            purchaseType = LocalSaveManager.saveData.shopPurchaseType;
            // 自動レイアウト+ScrollGridを使用するため強制的に計算をする
            LayoutRebuilder.ForceRebuildLayoutImmediate(adjuster);
            UpdatedBillingRewardBonusList();
        }
     
        private void UpdatedBillingRewardBonusList()
        {
            var packList = new List<ShopPackItem.Data>();
            foreach (var bonus in data)
            {
                if (purchaseType == ShopManager.PurchaseType.Point)
                {
                    var mBillingRewardAlternativePoint =
                        MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                            bonus.mBillingRewardId);
                    if (mBillingRewardAlternativePoint == null) continue;
                }
                if (purchaseType == ShopManager.PurchaseType.External)
                {
                    BillingRewardMasterObject mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId);
                    if (mBillingReward == null) continue;
                    
                    BillingRewardAlternativePointMasterObject mBillingRewardAlternativePoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(mBillingReward.id, PointMasterObject.PointType.ExternalPoint);
                    if (mBillingRewardAlternativePoint == null) continue;
                    externalItemUi.SetCount(mBillingRewardAlternativePoint.mPointId, UserDataManager.Instance.GetExpiryPointValue(mBillingRewardAlternativePoint.mPointId), true);
                }
                
                var value = new ShopPackItem.Data();
                value.bonus = bonus;
                value.isNew = ShopManager.HasNewBillingReward(bonus) ||
                              (bonus.buyLimit >= 0 && bonus.buyLimit > bonus.buyCount &&
                               MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId)
                                   ?.price == 0);
                value.stepUpGroupList = data.Where(stepUpBonus =>
                    bonus.stepGroup != 0 && stepUpBonus.stepGroup == bonus.stepGroup &&
                    stepUpBonus.mBillingRewardId != bonus.mBillingRewardId).ToList();
                value.isBan = isBanUser;
                value.OnSelectedBillingRewardBonus = selectedBillingRewardBonusId => OnSelectedBillingRewardBonus?.Invoke(selectedBillingRewardBonusId);
                value.onUpdateUi = () => OnUpdateUi?.Invoke();
                //シークレットタブでステップアップは運用する想定はないとのことなので一旦Falseを設定しておく
                value.isPlayStepUpLockEffect = false;
                value.PurchaseType = purchaseType;
                packList.Add(value);
            }
            scrollGrid.SetItems(packList);
            externalItemRoot.SetActive(purchaseType == ShopManager.PurchaseType.External);
            ShopManager.SaveConfirmedBillingRewardId(data);
        }
        
        public void OnClickPurchaseChange()
        {
            purchaseType = ShopManager.ChangePurchase(purchaseType, data);
            UpdatedBillingRewardBonusList();
        }
    }
    
    
}