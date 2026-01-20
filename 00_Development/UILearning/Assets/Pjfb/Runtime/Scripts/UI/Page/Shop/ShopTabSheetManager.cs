using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.Page;
using CruFramework.UI;
using JetBrains.Annotations;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using UnityEngine;
using Pjfb.Master;
using UnityEngine.UI;
using Logger = CruFramework.Logger;

namespace Pjfb.Shop
{
    
    [Serializable]
    public enum ShopTabSheetType
    {
        Limited,
        Secret,
        Beginner, 
        Pass, 
        Normal, 
        Exchange,
        None
    }
    
    public class ShopTabSheetManager : SheetManager<ShopTabSheetType>
    {
        [Serializable]
        public class TabUi
        {
            public ShopTabSheetType type;
            public SheetTab sheetTab;
            public GameObject badge;
        }
        
        [SerializeField]
        private List<TabUi> tabUiList = new List<TabUi>();
        public void UpdateTabUi(List<ShopTabSheetType> types)
        {
            tabUiList.ForEach(ui =>
            {
                var flag = types.Any(type => type == ui.type);
                ui.sheetTab.gameObject.SetActive(flag);
            });
        }

        public void UpdateTabBadge(ShopTabSheetType type, bool isActive)
        {
            foreach (var tabUi in tabUiList)
            {
                if(tabUi.type != type || tabUi.badge == null) continue;
                tabUi.badge.SetActive(isActive);
            }
        }
    }
    
    public abstract class ShopCategorySheet : Sheet
    {
        
        [SerializeField] protected ShopTabSheetType sheetType;
        [SerializeField] protected Image headlineImage;
        [SerializeField] protected Sprite headlineSprite;
        [SerializeField] protected Image headlineCharaImage;
        [SerializeField] protected Sprite headlineCharaSprite;
        [SerializeField] protected GameObject purchaseChangeButtonRoot;
        
        public ShopTabSheetType SheetType => sheetType;
        private BillingRewardBonusSubCategory selectSubCategory;
        protected List<BillingRewardBonusDetail> data;
        public Action<BillingRewardBonusSubCategory> OnClickSubCategory;
        public Action<long> OnSelectedBillingRewardBonus;
        public Action OnUpdateTabBadge;
        public Action OnUpdateUi;
        protected bool isBanUser;

        protected abstract void InitView(List<BillingRewardBonusDetail> billingRewardBonusList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.Gem , bool isBan = false);
        
        public void Init(List<BillingRewardBonusDetail> billingRewardBonusList, List<NativeApiSaleIntroduction> nativeApiSaleIntroductionList, BillingRewardBonusSubCategory subCategory = BillingRewardBonusSubCategory.Gem, bool isBan = false)
        {
            data = billingRewardBonusList;
            selectSubCategory = subCategory;
            isBanUser = isBan;
            if (purchaseChangeButtonRoot != null)
            {
                purchaseChangeButtonRoot.SetActive(ShopManager.HasExternalPoint(data) || ShopManager.CheckPurchaseChangeFlg());
            }
            InitView(data, selectSubCategory, isBanUser);
            headlineImage.sprite = headlineSprite;
            headlineCharaImage.sprite = headlineCharaSprite;
            Logger.Log($"Init:{sheetType}Sheet");
        }

        protected List<ShopPackBanner.Data> CreateImageBannerIds(Action onUpdateBadge)
        {
            var bannerList = new Dictionary<long, ShopPackBanner.Data>();

            for (var i = 0; i < data.Count; i++)
            {
                var rewardBonus = data[i];
                var imageNumber = rewardBonus.imageNumber;
                var isBadge = ShopManager.HasNewBillingReward(rewardBonus) ||
                              (rewardBonus.buyLimit >= 0 && rewardBonus.buyLimit > rewardBonus.buyCount &&
                               MasterManager.Instance.billingRewardMaster.FindData(rewardBonus.mBillingRewardId)
                                   ?.price == 0);

                if (!bannerList.ContainsKey(rewardBonus.imageNumber))
                {
                    bannerList[imageNumber] = new ShopPackBanner.Data();
                    bannerList[imageNumber].mBillingRewardBonusList = new List<BillingRewardBonusDetail>();
                    bannerList[imageNumber].SaleIntroductionList = new List<NativeApiSaleIntroduction>();
                }

                bannerList[imageNumber].mBillingRewardBonusList.Add(rewardBonus);

                if (isBadge)
                {
                    bannerList[imageNumber].isBadge = isBadge;
                }

                bannerList[imageNumber].isBan = isBanUser;
                bannerList[imageNumber].onUpdateBadge = () =>
                {
                    bannerList[imageNumber].isBadge = ShopManager.HasNewBillingReward(bannerList[imageNumber].mBillingRewardBonusList);
                    onUpdateBadge?.Invoke();
                };
                bannerList[imageNumber].OnSelectedBillingRewardBonus = selectedBillingRewardBonusId =>
                    OnSelectedBillingRewardBonus?.Invoke(selectedBillingRewardBonusId);
                bannerList[imageNumber].onUpdateUi = () => OnUpdateUi?.Invoke();
            }

            foreach (var banner in bannerList.Values)
            {
                var lastCloseReward = banner.mBillingRewardBonusList
                    .OrderBy(pack => ShopManager.GetDateTimeByString(pack.closedDatetime)).LastOrDefault();
                if(banner.mBillingRewardBonusList.Any(rewardBonus => rewardBonus.GetCategory() == BillingRewardBonusCategory.Secret))
                {
                    var lastCloseSaleIntroduction = banner.SaleIntroductionList
                        .OrderBy(saleIntroduction => saleIntroduction.expireAt.TryConvertToDateTime()).LastOrDefault();
                    banner.expireAt = lastCloseSaleIntroduction != null && lastCloseSaleIntroduction.expireAt.TryConvertToDateTime() != DateTime.MinValue 
                        ? lastCloseSaleIntroduction?.expireAt
                        : lastCloseReward?.closedDatetime ?? string.Empty;
                }
                else
                {
                    banner.expireAt = lastCloseReward?.closedDatetime ?? string.Empty;
                }
                var firstPriorityAppealTextReward = banner.mBillingRewardBonusList?.Where(bonus => !string.IsNullOrEmpty(bonus.appealText)).OrderByDescending(bonus => bonus.priority)
                    .ThenBy(bonus => bonus.mBillingRewardBonusId).FirstOrDefault();
                banner.appealText = firstPriorityAppealTextReward?.appealText ?? string.Empty;
            }
            return bannerList.Values.ToList();
        }

    }
}