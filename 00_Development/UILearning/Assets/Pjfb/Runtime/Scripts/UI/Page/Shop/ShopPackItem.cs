using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb.Shop
{

    public class ShopPackItem : ScrollGridItem
    {
        public class Data
        {
            public BillingRewardBonusDetail bonus;
            public bool isNew;
            public List<BillingRewardBonusDetail> stepUpGroupList;
            public bool isBan;
            public Action<long> OnSelectedBillingRewardBonus;
            public Action onUpdateUi;
            public bool isPlayStepUpLockEffect;
            public ShopManager.PurchaseType PurchaseType;
        }

        #region SerializeFields
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject requiredPointRoot;
        [SerializeField] private IconImage requiredPointImage;
        [SerializeField] private TMP_Text requiredPointValueText;
        [SerializeField] private IconImage externalPointImage;
        [SerializeField] private TMP_Text externalPointText;
        [SerializeField] private TMP_Text limitedText;
        [SerializeField] private GameObject limitedRoot;
        [SerializeField] private GameObject scheduleBaseRoot;
        [SerializeField] private TMP_Text scheduleText;
        [SerializeField] private GameObject scheduleRoot;
        [SerializeField] private TMP_Text specialText;
        [SerializeField] private GameObject specialRoot;
        [SerializeField] private TMP_Text buyButtonText;
        [SerializeField] private GameObject newBadge;
        [SerializeField] private ScrollGrid itemList;
        [SerializeField] private GameObject rockObject;
        [SerializeField] private TMP_Text rockText;
        [SerializeField] private Animator rockAnimator;
        [SerializeField] private GameObject coverObject;
        [SerializeField] private TMP_Text coverText;
        [SerializeField] private UIButton buyButton;
        [SerializeField] private GameObject soldOutRoot;
        [SerializeField] private GameObject scheduleSpecialCoverRoot;
        [SerializeField] private CallAnimationEventAction callAnimationEventAction; 

        #endregion

        private Data data;
        
        protected override void OnSetView(object value)
        {
            data = (Data) value;
            callAnimationEventAction.AnimationEventAction = () =>
            {
                if (scheduleSpecialCoverRoot != null) scheduleSpecialCoverRoot.SetActive(false);
            };
            
            var detail = data.bonus;
            nameText.text = detail.name;
            newBadge.SetActive(data.isNew);
            priceText.gameObject.SetActive(data.PurchaseType == ShopManager.PurchaseType.Price);
            requiredPointRoot.SetActive(data.PurchaseType == ShopManager.PurchaseType.Point);
            externalPointText.gameObject.SetActive(data.PurchaseType == ShopManager.PurchaseType.External);
            var mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(detail.mBillingRewardId);
            if (mBillingReward == null)
            {
                CruFramework.Logger.LogError($"mBillingRewardが取得できませんでした。　mBillingRewardId:{detail.mBillingRewardId}");
                return;
            }
            switch (data.PurchaseType)
            {
                case ShopManager.PurchaseType.Price:
                    var price = mBillingReward.price;
                    if (price != 0)
                    {
                        priceText.text = string.Format(StringValueAssetLoader.Instance["shop.price"], mBillingReward.price.GetStringNumberWithComma());
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.purchase_button"];
                    }
                    else
                    {
                        priceText.text = StringValueAssetLoader.Instance["shop.free"];
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.free.buy"];
                    }
                    break;
                case ShopManager.PurchaseType.Point:
                    var mBillingRewardAlternativePoint =
                        MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                            mBillingReward.id);
                    if (mBillingRewardAlternativePoint == null)
                    {
                        CruFramework.Logger.LogError($"mBillingRewardAlternativePointが取得できませんでした。　mBillingRewardId:{mBillingReward.id}");
                        break;
                    }
                    requiredPointImage.SetTexture(mBillingRewardAlternativePoint.mPointId);
                    requiredPointValueText.text = mBillingRewardAlternativePoint.value.ToString();
                    buyButtonText.text = StringValueAssetLoader.Instance["shop.exchange.button"];
                    break;
                case ShopManager.PurchaseType.External:
                    BillingRewardAlternativePointMasterObject mBillingRewardAlternative = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(mBillingReward.id, PointMasterObject.PointType.ExternalPoint);
                    if (mBillingRewardAlternative == null) break;
                    externalPointImage.SetTexture(mBillingRewardAlternative.mPointId);
                    if (mBillingRewardAlternative.value != 0)
                    {
                        externalPointText.text = mBillingRewardAlternative.value.GetStringNumberWithComma();
                    }
                    else
                    {
                        externalPointText.text = StringValueAssetLoader.Instance["shop.free"];
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.free.buy"];
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
            var isExistSpecialRoot = !string.IsNullOrEmpty(detail.appealText);
            specialRoot.SetActive(isExistSpecialRoot);
            specialText.text = detail.appealText;
            var closedDateTime = string.Empty;
            if (detail.GetCategory() == BillingRewardBonusCategory.Secret)
            {
                foreach (var saleIntroduction in ShopManager.SaleIntroductionList)
                {
                    var mSaleIntroduction =
                        MasterManager.Instance.saleIntroductionMaster.FindData(saleIntroduction.mSaleIntroductionId);
                    if(mSaleIntroduction == null || mSaleIntroduction.mBillingRewardBonusId != detail.mBillingRewardBonusId) continue;
                    closedDateTime = saleIntroduction.expireAt;
                    break;
                }
                
            }
            else
            {
                closedDateTime = detail.closedDatetime;
            }
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var remainTime = ShopManager.GetRemainTimeSpan(closedDateTime, AppTime.Now);
            var isExistScheduleRoot = !string.IsNullOrEmpty(closedDateTime) && remainTime.Days <= 365;
            scheduleRoot.SetActive(isExistScheduleRoot);
            scheduleText.text = ShopManager.GetShopRemainTimeString(remainTime);
            scheduleBaseRoot.SetActive(isExistSpecialRoot || isExistScheduleRoot);
            coverObject.SetActive(false);
            rockObject.SetActive(false);

            // 購入制限がある
            if (detail.buyLimit > 0)
            {
                var count = detail.buyLimit - detail.buyCount;
                var canBuy = count > 0;
                limitedRoot.SetActive(canBuy);
                limitedText.text = string.Format(StringValueAssetLoader.Instance["shop.remain.count"], count);
                buyButton.interactable = data.PurchaseType switch
                {
                    ShopManager.PurchaseType.Price => canBuy,
                    ShopManager.PurchaseType.Point => canBuy && CanPointExchange(),
                    ShopManager.PurchaseType.External => canBuy && CanExternalPointExchange(mBillingReward),
                    _ => throw new ArgumentOutOfRangeException()
                };
                soldOutRoot.SetActive(!canBuy);
                scheduleSpecialCoverRoot.SetActive(scheduleBaseRoot.activeSelf && !canBuy);
            }
            else
            {
                limitedRoot.SetActive(false);
                buyButton.interactable = true;
                buyButton.interactable = data.PurchaseType switch
                {
                    ShopManager.PurchaseType.Price => true,
                    ShopManager.PurchaseType.Point => CanPointExchange(),
                    ShopManager.PurchaseType.External => CanExternalPointExchange(mBillingReward),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                // 購入可能か
                bool isCanBuy = detail.IsPurchaseUnlimitedCanBuy();
                soldOutRoot.SetActive(isCanBuy == false);
                buyButton.interactable = isCanBuy;
                
                scheduleSpecialCoverRoot.SetActive(isCanBuy == false);
            }

            var prizeList = new List<ShopPackIconGridItem.Data>();

            if (mBillingReward.price > 0)
            {
                foreach (var prize in mBillingReward.prizeJson)
                {
                    var freeValue = prize.args.value - mBillingReward.paidPointValue;
                    var paidValue =  mBillingReward.paidPointValue;
                    if (freeValue > 0)
                    {
                        // 有償アイテム
                        if (paidValue > 0)
                        {
                            var paidPrize = new Master.PrizeJsonWrap(prize);
                            paidPrize.args.value = paidValue;
                            prizeList.Add(new ShopPackIconGridItem.Data(paidPrize,true));
                        }
                       
                        // 無償アイテム
                        var freePrize = new Master.PrizeJsonWrap(prize);
                        freePrize.args.value = freeValue;
                        prizeList.Add(new ShopPackIconGridItem.Data(freePrize,false));
                    }
                    else
                    {
                        // 有償アイテム
                        prizeList.Add(new ShopPackIconGridItem.Data(prize,true));
                    }
                }
            }
            
            foreach (var prize in detail.prizeJsonList)
            {
                prizeList.Add(new ShopPackIconGridItem.Data(prize));
            }
            
            itemList.SetItems(prizeList);
            
            if(soldOutRoot.activeSelf) return;

            // 返金で課金制限がかかっていた場合の制限表示（無料パックは除く）
            if (data.isBan && mBillingReward.price > 0)
            {
                coverObject.SetActive(true);
                rockObject.SetActive(false);
                coverText.text = StringValueAssetLoader.Instance["shop.purchase.limit_ban"];
                scheduleSpecialCoverRoot.SetActive(true);
                return;
            }
            
            // 課金上限の制限表示
            if (data.PurchaseType != ShopManager.PurchaseType.External)
            {
                if (!coverObject.activeSelf && UserDataManager.Instance.user.hasRegisteredBirthday)
                {
                    var monthPayment = UserDataManager.Instance.user.monthPayment;
                    var monthPaymentLimit = UserDataManager.Instance.user.monthPaymentLimit;
                    if (mBillingReward.price + monthPayment > monthPaymentLimit)
                    {
                        coverObject.SetActive(true);
                        rockObject.SetActive(false);
                        coverText.text = StringValueAssetLoader.Instance["shop.purchase.limit"];
                        scheduleSpecialCoverRoot.SetActive(true);
                        return;
                    }
                }
            }

            // ステップアップの制限表示
            if (data.stepUpGroupList != null && data.stepUpGroupList.Count > 0)
            {
                var unlockReqBonus = data.stepUpGroupList.Where(groupBonus => groupBonus.stepNumber < detail.stepNumber).OrderByDescending(groupBonus => groupBonus.stepNumber).FirstOrDefault();
                var isStepUpLocked = detail.IsStepUpLocked(data.stepUpGroupList);
                coverObject.SetActive(false);
                var isStepUpUnLockEffect = !isStepUpLocked && unlockReqBonus != null &&
                                           ShopManager.HasStepUpUnLockEffectBillingReward(data.bonus
                                               .mBillingRewardBonusId);
                var isActiveLock = isStepUpUnLockEffect ||
                                   isStepUpLocked && unlockReqBonus != null;
                rockObject.SetActive(isActiveLock);
                scheduleSpecialCoverRoot.SetActive(isActiveLock);
                if (isActiveLock)
                {
                    rockText.text = string.Format(StringValueAssetLoader.Instance["shop.stepup.lock"],unlockReqBonus.name);
                }
                if (data.isPlayStepUpLockEffect && isStepUpUnLockEffect)
                {
                    ShopManager.SaveConfirmedStepUpUnLockEffectBillingRewardId(data.bonus);
                    StepUpLockEffectAsync().Forget();
                }
                if (isStepUpLocked)
                {
                    limitedText.text = string.Format(StringValueAssetLoader.Instance["shop.remain.count"], detail.buyLimit);
                    limitedText.gameObject.SetActive(detail.buyLimit > 0);
                    buyButton.interactable = false;
                }
            }
            else
            {
                coverObject.SetActive(false);
                rockObject.SetActive(false);
            }
        }

        private bool CanPointExchange()
        {
            var mBillingRewardAlternativePoint =
                MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                    data.bonus.mBillingRewardId);
            if (mBillingRewardAlternativePoint == null)
            {
                CruFramework.Logger.LogError($"mBillingRewardAlternativePointが取得できませんでした。　mBillingRewardId:{data.bonus.mBillingRewardId}");
                return false;
            }
            var point = UserDataManager.Instance.point.Find(mBillingRewardAlternativePoint.mPointId);
            return (point?.value ?? 0) >= mBillingRewardAlternativePoint.value;
        }

        // 外部決済アイテムで購入できるかどうか
        private bool CanExternalPointExchange(BillingRewardMasterObject billingMasterObject)
        {
            BillingRewardAlternativePointMasterObject billingRewardAlternativePoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(billingMasterObject.id, PointMasterObject.PointType.ExternalPoint);
            if(billingRewardAlternativePoint == null) return false;
            // 期限内のアイテム数で判断
            long pointValue = UserDataManager.Instance.GetExpiryPointValue(billingRewardAlternativePoint.mPointId);
            return pointValue >= billingRewardAlternativePoint.value;
        }
        public void OnClickPurchaseButton()
        {
            ShopManager.OpenPurchaseConfirm(data.bonus, data.PurchaseType, data.onUpdateUi).Forget();
            data.OnSelectedBillingRewardBonus?.Invoke(data.bonus.mBillingRewardBonusId);
        }


        private async UniTask StepUpLockEffectAsync()
        {
            await AnimatorUtility.WaitStateAsync(rockAnimator, "Unlocked");
            if(rockObject != null)　rockObject.SetActive(false);
        }
    }
}