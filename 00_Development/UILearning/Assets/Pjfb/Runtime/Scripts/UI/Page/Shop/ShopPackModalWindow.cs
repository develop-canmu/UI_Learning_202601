using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb.Shop
{
    public class ShopPackModalWindow : ModalWindow
    {
        public class Data
        {
            public List<BillingRewardBonusDetail> billingRewardBonusList;
            public bool isBan;
            public Action<long> OnSelectedBillingRewardBonus;
            public Action onUpdateUi;
            public Action OnUpdateBadge;
            public Action OnRefreshUI { get; set; }
        }
        
        [SerializeField] private ScrollGrid packScrollGrid = null;
        [SerializeField] private ShopPackBanner shopPackBanner;
        [SerializeField] private GameObject purchaseChangeButtonRoot;
        [SerializeField] private PossessionItemUi externalItemUi;
        [SerializeField] private RectTransform adjuster;
        private Data data;
        private List<ShopPackItem.Data> packList = new();
        private ShopManager.PurchaseType purchaseType;

        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopPack, data);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            var imageNumber = data.billingRewardBonusList?.FirstOrDefault()?.imageNumber ?? 0;
            if (data?.billingRewardBonusList == null) return;
            
            UpdatedBillingRewardBonusList();
            purchaseChangeButtonRoot.SetActive(ShopManager.HasExternalPoint(data.billingRewardBonusList) || ShopManager.CheckPurchaseChangeFlg());
            await shopPackBanner.SetBanner(imageNumber);
        }
        
        protected override void OnOpened()
        {
            foreach (var pack in packList)
            {
                var bonus = data.billingRewardBonusList.FirstOrDefault(data =>
                    data.mBillingRewardBonusId == pack.bonus.mBillingRewardBonusId);
                if(bonus == null) continue;
                pack.isNew = ShopManager.HasNewBillingReward(bonus) ||
                             (bonus.buyLimit >= 0 && bonus.buyLimit > bonus.buyCount &&
                              MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId)?.price ==
                              0);
                pack.isPlayStepUpLockEffect = true;
            }
            packScrollGrid.RefreshItemView();
            data.OnUpdateBadge?.Invoke();
        }

        private void UpdatedBillingRewardBonusList()
        {
            packList.Clear();
            // 購入方法を取得
            purchaseType = LocalSaveManager.saveData.shopPurchaseType;
            foreach (var bonus in data.billingRewardBonusList)
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
                value.stepUpGroupList = data.billingRewardBonusList
                    .Where(stepUpBonus => bonus.stepGroup != 0 && stepUpBonus.stepGroup == bonus.stepGroup &&
                                          stepUpBonus.mBillingRewardBonusId != bonus.mBillingRewardBonusId).ToList();
                value.isBan = data.isBan;
                value.OnSelectedBillingRewardBonus = selectedBillingRewardBonusId =>
                    data.OnSelectedBillingRewardBonus?.Invoke(selectedBillingRewardBonusId);
                value.onUpdateUi = OnUpdateUI;
                value.isPlayStepUpLockEffect = false;
                value.PurchaseType = purchaseType;
                packList.Add(value);
            }
            externalItemUi.gameObject.SetActive(purchaseType == ShopManager.PurchaseType.External);
            // 自動レイアウト+ScrollGridを使用するため強制的に計算をする
            LayoutRebuilder.ForceRebuildLayoutImmediate(adjuster);
            
            packScrollGrid.SetItems(packList);
        }
        
        #region EventListeners

        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        
        public void OnClickTermsPaymentLaw()
        {
            PaymentLawModal.Open();
        }

        public void OnClickCloseButton()
        {
            // 購入制限反映のためリストをリフレッシュする
            data.OnRefreshUI?.Invoke();
            Close();
        }
        
        public void OnClickPurchaseChange()
        {
            purchaseType = ShopManager.ChangePurchase(purchaseType, data.billingRewardBonusList);
            UpdatedBillingRewardBonusList();
        }
        
        private void OnUpdateUI()
        {
            // 購入方法をモーダル外で変更した場合の処理
            data.onUpdateUi?.Invoke();
            UpdatedBillingRewardBonusList();
        }

        #endregion
    }
}
