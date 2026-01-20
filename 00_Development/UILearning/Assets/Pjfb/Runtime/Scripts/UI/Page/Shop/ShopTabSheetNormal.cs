using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.Page;
using UnityEngine;
using UnityEngine.UI;

using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UI;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb.Shop
{

    public class ShopTabSheetNormal : ShopCategorySheet
    {
        [Serializable]
        public class SubCategoryToggle
        {
            public BillingRewardBonusSubCategory subCategory;
            public TextToggleItem toggle;
            public GameObject badge;
        }
        
        [SerializeField] private ScrollGrid scrollGrid = null;
        [SerializeField] private ToggleContainer SubtabToggleContainer;
        [SerializeField] private SubCategoryToggle[] subCategoryToggleList;
        [SerializeField] private GameObject externalItemRoot;
        [SerializeField] private PossessionItemUi externalItemUi;
        [SerializeField] private RectTransform adjuster;
        private BillingRewardBonusSubCategory selectBonusSubCategory;
        private ShopManager.PurchaseType purchaseType;
      
        protected override void InitView(List<BillingRewardBonusDetail> billingRewardBonusList,
            BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.Gem, bool isBan = false)
        {
            Logger.Log("TODO 常設タブ:" + billingRewardBonusList.Count);
            data = billingRewardBonusList;
            isBanUser = isBan;
            
            selectBonusSubCategory = UpdateSubCategoryTabUi(subCategory);
            purchaseType = LocalSaveManager.saveData.shopPurchaseType;
            // 自動レイアウト+ScrollGridを使用するため強制的に計算をする
            LayoutRebuilder.ForceRebuildLayoutImmediate(adjuster);
            UpdatedBillingRewardBonusList();
            var index = 0;
            for (var i = 0; i < subCategoryToggleList.Length; i++)
            {
                if (subCategoryToggleList[i].subCategory == selectBonusSubCategory)
                {
                    index = i;
                    break;
                }
            }
            SubtabToggleContainer.Init(initialIndexDisplay: index, onSelectIndex: OnClickToggle);
        }

        private BillingRewardBonusSubCategory UpdateSubCategoryTabUi(BillingRewardBonusSubCategory category)
        {
            var availableTab = BillingRewardBonusSubCategory.NoUse;
            foreach (var subCategoryToggle in subCategoryToggleList)
            {
                // カテゴリーに指定された商品がないのであればタブ非表示
                var isActive = data.Any(reward => reward.GetSubCategory() == subCategoryToggle.subCategory);
                subCategoryToggle.toggle.gameObject.SetActive(isActive);
                if (isActive && availableTab != category)
                {
                    availableTab = subCategoryToggle.subCategory;
                }
            }
            return availableTab;
        }
        
        private void UpdatedBillingRewardBonusList()
        {
            var billingRewardBonusList = data.Where(reward => reward.GetSubCategory() == selectBonusSubCategory).ToList();
            Logger.Log($"TODO{selectBonusSubCategory}:" + billingRewardBonusList.Count);
            var packList = new List<ShopPackItem.Data>();
            foreach (var bonus in billingRewardBonusList)
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
                value.stepUpGroupList = billingRewardBonusList
                    .Where(stepUpBonus => bonus.stepGroup != 0 && stepUpBonus.stepGroup == bonus.stepGroup && stepUpBonus.mBillingRewardId != bonus.mBillingRewardId).ToList();
                value.isBan = isBanUser;
                value.OnSelectedBillingRewardBonus = selectedBillingRewardBonusId => OnSelectedBillingRewardBonus?.Invoke(selectedBillingRewardBonusId);
                value.onUpdateUi = () => OnUpdateUi?.Invoke();
                //常設タブでステップアップは運用する想定はまだないとのことなので一旦Falseを設定しておく
                value.isPlayStepUpLockEffect = false;
                value.PurchaseType = purchaseType;
                packList.Add(value);
            }
            OnClickSubCategory?.Invoke(selectBonusSubCategory);
            scrollGrid.SetItems(packList);
            SetBadge();
            externalItemRoot.SetActive(purchaseType == ShopManager.PurchaseType.External);
            ShopManager.SaveConfirmedBillingRewardId(billingRewardBonusList);
        }

        private void SetBadge()
        {
            OnUpdateTabBadge?.Invoke();
            
            foreach (var subCategoryToggle in subCategoryToggleList)
            {
                subCategoryToggle.badge.SetActive(false);
                var billingRewardBonusList = data.Where(reward => reward.GetSubCategory() == subCategoryToggle.subCategory).ToList();
                foreach (var bonus in billingRewardBonusList)
                {
                    var mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId);
                    var count = bonus.buyLimit - bonus.buyCount;
                    if (!ShopManager.HasNewBillingReward(bonus) && (mBillingReward.price != 0 || count <= 0)) continue;
                    subCategoryToggle.badge.SetActive(true);
                    break;
                }
            }
        }

        private async UniTask OnClickToggle(int index)
        {
            selectBonusSubCategory = subCategoryToggleList[index].subCategory;
            UpdatedBillingRewardBonusList();
            await UniTask.NextFrame();
        }

        public void OnClickPurchaseChange()
        {
            purchaseType = ShopManager.ChangePurchase(purchaseType,data);
            UpdatedBillingRewardBonusList();
        }
    }
}