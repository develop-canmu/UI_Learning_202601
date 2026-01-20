using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;
using TMPro;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine.Analytics;

namespace Pjfb.Shop
{

    public class ShopPackBanner : ScrollGridItem
    {
        #region Params
        public class Data
        {
            public List<BillingRewardBonusDetail> mBillingRewardBonusList;
            public List<NativeApiSaleIntroduction> SaleIntroductionList;
            public bool isBadge;
            public string appealText;
            public string expireAt;
            public bool isBan;
            public Action onUpdateBadge;
            public Action<long> OnSelectedBillingRewardBonus;
            public Action onUpdateUi;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text scheduleText;
        [SerializeField] private GameObject scheduleBaseRoot;
        [SerializeField] private GameObject scheduleRoot;
        [SerializeField] private TMP_Text specialText;
        [SerializeField] private GameObject specialRoot;
        [SerializeField] private GameObject badgeItem;
        [SerializeField] private CancellableWebTexture webTexture;
        [SerializeField] private GameObject soldOutRoot;
        [SerializeField] private GameObject scheduleSpecialCoverRoot;
        [SerializeField] private GameObject coverObject;
        [SerializeField] private TMP_Text coverText;
        #endregion

        public Data data;

        protected override void OnSetView(object value)
        {
            data = (Data) value;
            badgeItem.SetActive(data.isBadge);

            var imageNumber = data.mBillingRewardBonusList?.FirstOrDefault()?.imageNumber ?? 0;
            SetBanner(imageNumber).Forget();
            
            var isExistSpecialRoot = !string.IsNullOrEmpty(data.appealText);
            specialRoot.SetActive(isExistSpecialRoot);
            specialText.text = data.appealText;
            var closedDateTime = data.expireAt;
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var remainTime = ShopManager.GetRemainTimeSpan(closedDateTime, AppTime.Now);
            var isExistScheduleRoot = !string.IsNullOrEmpty(closedDateTime) && remainTime.Days <= 365;
            scheduleRoot.SetActive(isExistScheduleRoot);
            scheduleText.text = ShopManager.GetShopRemainTimeString(remainTime);
            scheduleBaseRoot.SetActive(isExistSpecialRoot || isExistScheduleRoot);

            soldOutRoot.SetActive(data.mBillingRewardBonusList != null && data.mBillingRewardBonusList.All(bonus => bonus.buyLimit > 0 && bonus.buyLimit - bonus.buyCount <= 0));
            scheduleSpecialCoverRoot.SetActive(data.mBillingRewardBonusList != null && data.mBillingRewardBonusList.All(bonus => bonus.buyLimit > 0 && bonus.buyLimit - bonus.buyCount <= 0));
            coverObject.SetActive(false);
            
            if(soldOutRoot.activeSelf) return;
            
            // 返金で課金制限がかかっていた場合の制限表示
            if (data.isBan)
            {
                coverObject.SetActive(true);
                coverText.text = StringValueAssetLoader.Instance["shop.purchase.limit_ban"];
                scheduleSpecialCoverRoot.SetActive(true);
                return;
            }

            ShopManager.PurchaseType purchaseType = LocalSaveManager.saveData.shopPurchaseType;
            // 購入制限がかかっている場合は購入制限をかける
            if (purchaseType != ShopManager.PurchaseType.External)
            {
                if (!coverObject.activeSelf && UserDataManager.Instance.user.hasRegisteredBirthday)
                {
                    var monthPayment = UserDataManager.Instance.user.monthPayment;
                    var monthPaymentLimit = UserDataManager.Instance.user.monthPaymentLimit;
                    if (data.mBillingRewardBonusList == null) return;
                    // バナー内の全てのパックが購入制限に該当する場合バナーをグレーアウトする
                    if (data.mBillingRewardBonusList.All(bonus =>
                        {
                            var mBillingReward =
                                MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId);
                            return mBillingReward.price + monthPayment > monthPaymentLimit;
                        }))
                    {
                        coverObject.SetActive(true);
                        coverText.text = StringValueAssetLoader.Instance["shop.purchase.limit"];
                        scheduleSpecialCoverRoot.SetActive(true);
                    }
                }
            }
        }
        
        public async UniTask SetBanner(long imageNumber)
        {
            var key = ShopManager.GetPackBannerPath(imageNumber);
            await webTexture.SetTextureAsync(key);
        }
        
        public void OnClickPackBannerButton()
        {
            var modalData = new ShopPackModalWindow.Data
            {
                billingRewardBonusList = data.mBillingRewardBonusList,
                isBan = data.isBan,
                OnSelectedBillingRewardBonus = selectedBillingRewardBonusId => data.OnSelectedBillingRewardBonus(selectedBillingRewardBonusId),
                onUpdateUi = () => data.onUpdateUi?.Invoke(),
                OnUpdateBadge = () =>
                {
                    ShopManager.SaveConfirmedBillingRewardId(data.mBillingRewardBonusList);
                    data.onUpdateBadge?.Invoke();
                },
                OnRefreshUI = () => ParentScrollGrid.RefreshItemView()
            };
            ShopPackModalWindow.Open(modalData);
        }
    }
}