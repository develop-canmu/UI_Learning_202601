using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using UnityEngine;
using System.Linq;
using Pjfb.Networking.App;


namespace Pjfb.Shop
{
    public class ShopPageArgs
    {
        public ShopTabSheetType selectedTab = ShopTabSheetType.None;
        public int mBillingRewardBonusId;
    }

    public class ShopPage : Page
    {
        [SerializeField] private ShopTabSheetManager sheetManager = null;
        [SerializeField] private List<ShopCategorySheet> shopSheetList;

        private BillingRewardBonusDetail[] availableRewardBonus;
        private BillingRewardBonusDetail[] defaultAvailableRewardBonus;
        private List<ShopTabSheetType> availableShopTabType = new List<ShopTabSheetType>();

        private ShopPageArgs data;
        private ShopTabSheetType selectedTab;
        private long selectedBillingRewardBonusId;
        private BillingRewardBonusSubCategory selectedSubCategory;

        private long freeBillingRewardBonusCount;
        private long paymentPenaltyLevel;

        private bool isSaveSoldOutPack = false;
        private List<long> visibleSoldOutRewardBonusIdList = new List<long>();

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (ShopPageArgs) args;
            sheetManager.OnOpenSheet += OnOpenSheet;
            // 購入後のシークレットバナーの自動表示を無効化
            ShopManager.UnregistrationOpenSecretBannerModal();
            ShopManager.OnPurchaseSuccess += OnPurchaseSuccess;
            selectedBillingRewardBonusId = data?.mBillingRewardBonusId ?? 0;
            selectedSubCategory = BillingRewardBonusSubCategory.Gem;
            selectedTab = data?.selectedTab ?? ShopTabSheetType.Limited;
            visibleSoldOutRewardBonusIdList.Clear();
            
            var limitedSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Limited);
            if (limitedSheet != null)
            {
                limitedSheet.OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus;
                limitedSheet.OnUpdateTabBadge = OnUpdateTabBadge;
                limitedSheet.OnUpdateUi = () => sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            }
            
            var secretSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Secret);
            if (secretSheet != null)
            {
                secretSheet.OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus;
                secretSheet.OnUpdateTabBadge = OnUpdateTabBadge;
                secretSheet.OnUpdateUi = () => sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            }
            
            var beginnerSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Beginner);
            if (beginnerSheet != null)
            {
                beginnerSheet.OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus;
                beginnerSheet.OnUpdateTabBadge = OnUpdateTabBadge;
                beginnerSheet.OnUpdateUi = () => sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            }
            
            var passSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Pass);
            if (passSheet != null)
            {
                passSheet.OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus;
                passSheet.OnUpdateTabBadge = OnUpdateTabBadge;
                passSheet.OnUpdateUi = () => sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            }
            
            var normalSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Normal);
            if (normalSheet != null)
            {
                normalSheet.OnClickSubCategory = OnClickSubCategory;
                normalSheet.OnUpdateTabBadge = OnUpdateTabBadge;
                normalSheet.OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus;
                normalSheet.OnUpdateUi = () => sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            }
            
            var exchangeSheet = shopSheetList.FirstOrDefault(sheet => sheet.SheetType == ShopTabSheetType.Exchange);
            if (exchangeSheet != null)
            {
                exchangeSheet.OnUpdateTabBadge = OnUpdateTabBadge;
            }

            var isSuccess = await InitializeUi();
            if (!isSuccess) return;
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnDestroy()
        {
            sheetManager.OnOpenSheet -= OnOpenSheet;
            ShopManager.OnPurchaseSuccess -= OnPurchaseSuccess;
            base.OnDestroy();
        }

        private bool IsApiError(long errorCode)
        {
            return errorCode != 0 && errorCode != (long)APIResponseCode.ShopMaintenance;
        }

        private bool IsShopMaintenance(long errorCode)
        {
            return errorCode == (long)APIResponseCode.ShopMaintenance;
        }

        private async UniTask<bool> InitializeUi()
        {
            var errorCode = await GetShopGetBillingRewardBonusListAPI();
            if (IsApiError(errorCode))
            {
                return false;
            }
            
            // 売り切れのパックを非表示にするためのデータ更新
            UpdateAvailableRewardBonusListBySoldOut();

            UpdateAvailableShopSheetType();

            if (IsShopMaintenance(errorCode))
            {
                selectedTab = ShopTabSheetType.Exchange;
                var bonusList = GetAvailableRewardBonusList(selectedTab);
                sheetManager.OpenSheet(selectedTab, bonusList, true);
                sheetManager.UpdateTabBadge(selectedTab, ShopManager.HasNewBillingReward(bonusList));
                return false;
            }
            
            AppManager.Instance.UIManager.Footer.ShopButton.SetNotificationBadge(
                ShopManager.HasNewBillingReward(availableRewardBonus.ToList()) || freeBillingRewardBonusCount > 0);
            
            if (selectedBillingRewardBonusId != 0)
            {
                var billingRewardBonus =
                    availableRewardBonus.FirstOrDefault(
                        bonus => bonus.mBillingRewardBonusId == selectedBillingRewardBonusId);
                var category = billingRewardBonus?.GetCategory() ?? BillingRewardBonusCategory.None;
                selectedTab = ConvertCategoryToSheetType(category);
            }
            
            if (!availableShopTabType.Contains(selectedTab))
            {
                selectedTab = availableShopTabType.FirstOrDefault();
            }
            
            CruFramework.Logger.Log($"ショップ遷移{selectedTab}/{selectedBillingRewardBonusId}/{selectedSubCategory}");

            sheetManager.OpenSheet(selectedTab, GetAvailableRewardBonusList(selectedTab), true);
            
            if (selectedTab == ShopTabSheetType.Limited && selectedBillingRewardBonusId != 0 ||
                selectedTab == ShopTabSheetType.Beginner && selectedBillingRewardBonusId != 0)
            {
                // 売り切れ等関係なく選択したボーナスを取得する
                // (現在購入できるボーナスリストからの取得だと売り切れていた場合などで取得できないことがあるため)
                var selectedBillingRewardBonus = defaultAvailableRewardBonus.FirstOrDefault(bonus =>
                    bonus.mBillingRewardBonusId == selectedBillingRewardBonusId);
                // 現在購入できるボーナスリストから選択したボーナスに紐づくimageNumberでボーナスリストを取得する
                var selectedBillingRewardBonusList = GetAvailableRewardBonusList(selectedTab).Where(bonus =>
                    bonus.imageNumber == (selectedBillingRewardBonus?.imageNumber ?? 0));
                // 取得したボーナスリストが空でない場合ShopPackModalWindowを開く
                if (selectedBillingRewardBonusList.Any())
                {
                    var modalData = new ShopPackModalWindow.Data
                    {
                        billingRewardBonusList = GetAvailableRewardBonusList(selectedTab).Where(bonus => bonus.imageNumber == (selectedBillingRewardBonus?.imageNumber ?? 0)).ToList(),
                        isBan = paymentPenaltyLevel >= ShopManager.BanPaymentPenaltyLevel,
                        OnSelectedBillingRewardBonus = OnSelectedBillingRewardBonus,
                        onUpdateUi = () => InitializeUi().Forget()
                    };
                    ShopPackModalWindow.Open(modalData);
                }
            }
           
            foreach (var shopTabSheetType in availableShopTabType)
            {
                sheetManager.UpdateTabBadge(shopTabSheetType, ShopManager.HasNewBillingReward(GetAvailableRewardBonusList(shopTabSheetType)));
            }
            
            // ペンディング確認(正否にかかわらずショップに居残るので初期化の最後で実行)
            ShopManager.CheckAndConfirmPendingProduct();

            return true;
            
        }

        #endregion
        
        #region EventListener
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        
        public void OnClickTermsPaymentLaw()
        {
            PaymentLawModal.Open();
        }

        private void OnPurchaseSuccess()
        {
            isSaveSoldOutPack = true;
            InitializeUi().Forget();
        }

        private void OnSelectedBillingRewardBonus(long billingRewardBonusId)
        {
            selectedBillingRewardBonusId = billingRewardBonusId;
        }

        private void OnClickSubCategory(BillingRewardBonusSubCategory subCategory)
        {
            selectedSubCategory = subCategory;
        }
        
        private void OnUpdateTabBadge()
        {
            sheetManager.UpdateTabBadge(selectedTab, ShopManager.HasNewBillingReward(GetAvailableRewardBonusList(selectedTab)));
            AppManager.Instance.UIManager.Footer.ShopButton.SetNotificationBadge(
                ShopManager.HasNewBillingReward(availableRewardBonus.ToList()) || freeBillingRewardBonusCount > 0);
        }

        // 外部決済サイトへの誘導ボタン
        public void OnClickExternalButton()
        {
            ShopManager.OpenExternalPurchaseSite();
        }
        
        #endregion
        
        #region API
        public static async UniTask<List<StoreBuyingInfo>> GetShopGetStoreBuyingInfoAPI()
        {
            ShopGetStoreBuyingInfoAPIRequest request = new ShopGetStoreBuyingInfoAPIRequest();
            await APIManager.Instance.Connect(request);
            ShopGetStoreBuyingInfoAPIResponse response = request.GetResponseData();
            return response.storeBuyingInfoList.ToList();
        }
        
        private async UniTask<long> GetShopGetBillingRewardBonusListAPI()
        {
            try
            {
                var response = await ShopManager.GetShopGetBillingRewardBonusListAPI();
                availableRewardBonus = response.billingRewardBonusList;
                // availableRewardBonusは売り切れ等でデータを削除するため変更しない元のデータをdefaultAvailableRewardBonusで保存する
                defaultAvailableRewardBonus = response.billingRewardBonusList;
                paymentPenaltyLevel = response.paymentPenaltyLevel;
            }
            catch(APIException e)
            {
                availableRewardBonus = Array.Empty<BillingRewardBonusDetail>();
                CruFramework.Logger.LogWarning($"ShopGetBillingRewardBonusListAPI error {e.errorParam}");
                return e.errorParam;
            }
            return 0;
        }

        #endregion

        private void UpdateAvailableShopSheetType()
        {
            availableShopTabType.Clear();
            foreach (var bonus in availableRewardBonus)
            {
                var category = bonus.GetCategory();
                var sheetType = ConvertCategoryToSheetType(category);
                if (sheetType == ShopTabSheetType.None)
                {
                    continue;
                }
                if (availableShopTabType.Contains(sheetType))
                {
                    continue;
                }
                availableShopTabType.Add(sheetType);
            }
            var availableCommonStore = MasterManager.Instance.commonStoreCategoryMaster.GetAvailableCommonStoreCategory();
            if (availableCommonStore != null && availableCommonStore.Any())
            {
                availableShopTabType.Add(ShopTabSheetType.Exchange);
            }
            availableShopTabType = availableShopTabType.OrderBy(type => type).ToList();
            sheetManager.UpdateTabUi(availableShopTabType);
        }

        private ShopTabSheetType ConvertCategoryToSheetType(BillingRewardBonusCategory category)
        {
            switch (category)
            {
                case BillingRewardBonusCategory.Normal:
                    return ShopTabSheetType.Normal;
                case BillingRewardBonusCategory.Pass:
                case BillingRewardBonusCategory.BattlePass:
                case BillingRewardBonusCategory.QuestBattlePass:
                    return ShopTabSheetType.Pass;
                case BillingRewardBonusCategory.Recommend:
                    return ShopTabSheetType.Limited;
                case BillingRewardBonusCategory.Beginner:
                    return ShopTabSheetType.Beginner;
                case BillingRewardBonusCategory.Secret:
                    return ShopTabSheetType.Secret;
                default:
                    return ShopTabSheetType.None;
            }
        }
        
        private void OnOpenSheet(ShopTabSheetType type)
        {
            sheetManager.UpdateTabBadge(selectedTab, ShopManager.HasNewBillingReward(GetAvailableRewardBonusList(selectedTab)));
            var sheet =  (ShopCategorySheet)sheetManager.CurrentSheet;
            selectedTab = type;
            sheet.Init(GetAvailableRewardBonusList(type), ShopManager.SaleIntroductionList, selectedSubCategory,paymentPenaltyLevel >= ShopManager.BanPaymentPenaltyLevel);
        }
        
        private List<BillingRewardBonusDetail> GetAvailableRewardBonusList(ShopTabSheetType type)
        {
            if (type == ShopTabSheetType.Exchange)
            {
                return new List<BillingRewardBonusDetail>();
            }
            
            var packagesIEnumerable = availableRewardBonus
                .Where(bonus =>
                {
                    var category = bonus.GetCategory();
                    switch (type)
                    {
                        case ShopTabSheetType.Limited:
                            return category == BillingRewardBonusCategory.Recommend;
                        case ShopTabSheetType.Secret:
                            return category == BillingRewardBonusCategory.Secret;
                        case ShopTabSheetType.Beginner:
                            return category == BillingRewardBonusCategory.Beginner;
                        case ShopTabSheetType.Pass:
                            return category == BillingRewardBonusCategory.Pass ||
                                   category == BillingRewardBonusCategory.BattlePass ||
                                   category == BillingRewardBonusCategory.QuestBattlePass;
                        case ShopTabSheetType.Normal:
                            return category == BillingRewardBonusCategory.Normal;
                        default:
                            return false;
                    }
                });

            var packages = packagesIEnumerable
                .OrderByDescending(bonus => bonus.buyLimit == 0 || bonus.buyLimit - bonus.buyCount > 0)
                .ThenByDescending(bonus => MasterManager.Instance.billingRewardMaster.FindData(bonus.mBillingRewardId)?.price == 0)
                .ThenBy(bonus => 
                {
                    var sameGroupBonus = packagesIEnumerable.Where(sameBonus => sameBonus.stepGroup != 0 && sameBonus.stepGroup == bonus.stepGroup && sameBonus.mBillingRewardBonusId != bonus.mBillingRewardBonusId);
                    return bonus.IsStepUpLocked(sameGroupBonus);
                })
                .ThenByDescending(bonus => bonus.priority)
                .ToList();
            
            return packages;
        }

        private void SetVisibleSoldOutRewardBonusIdList()
        {
            // パック購入後ではないまたはselectedBillingRewardBonusIdが0の場合はreturn
            if (!isSaveSoldOutPack || selectedBillingRewardBonusId == 0) return;
            // 再度通らないように通った時点でフラグは落としておく
            isSaveSoldOutPack = false;
            var billingRewardBonus = availableRewardBonus.FirstOrDefault(bonus =>
                bonus.mBillingRewardBonusId == selectedBillingRewardBonusId);
            // billingRewardBonusがnullもしくは購入できるパックの場合はreturn
            if (billingRewardBonus == null || billingRewardBonus.IsPurchaseUnlimitedCanBuy() ||
                billingRewardBonus.buyLimit - billingRewardBonus.buyCount > 0) return;
            var billingRewardBonusCategory = billingRewardBonus.GetCategory();
            var shopTabSheetType = ConvertCategoryToSheetType(billingRewardBonusCategory);
            // shopTabSheetTypeがNone,Pass,Exchangeの場合はreturn
            if (shopTabSheetType == ShopTabSheetType.None ||
                shopTabSheetType == ShopTabSheetType.Pass ||
                shopTabSheetType == ShopTabSheetType.Exchange) return;
            visibleSoldOutRewardBonusIdList.Add(selectedBillingRewardBonusId);
        }

        private void UpdateAvailableRewardBonusListBySoldOut()
        {
            SetVisibleSoldOutRewardBonusIdList();

            // 要素削除のため一時的にリストに変換
            var tempList = availableRewardBonus.ToList();
            for (var i = 0; i < tempList.Count;)
            {
                var bonus = tempList[i];
                var billingRewardBonusCategory = bonus.GetCategory();
                var shopTabSheetType = ConvertCategoryToSheetType(billingRewardBonusCategory);
                // None,Pass,Exchangeの場合continue
                if (shopTabSheetType == ShopTabSheetType.None ||
                    shopTabSheetType == ShopTabSheetType.Pass ||
                    shopTabSheetType == ShopTabSheetType.Exchange)
                {
                    i++;
                    continue;
                }
                // 購入できるパックの場合はcontinue
                if (bonus.IsPurchaseUnlimitedCanBuy() || bonus.buyLimit - bonus.buyCount > 0)
                {
                    i++;
                    continue;
                }
                // 表示する売り切れのパックの場合はcontinue
                if (visibleSoldOutRewardBonusIdList.Any(id => id == bonus.mBillingRewardBonusId)) 
                {
                    i++;
                    continue;
                }
                // 売り切れのパックを削除
                tempList.Remove(bonus);
            }

            // 該当の売り切れを削除したリストを代入
            availableRewardBonus = tempList.ToArray();
        }
    }
}
