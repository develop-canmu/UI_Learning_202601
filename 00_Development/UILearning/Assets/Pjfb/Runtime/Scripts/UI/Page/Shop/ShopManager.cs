using System;
using Logger = CruFramework.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Pjfb.Master;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine.Purchasing;
using UnityEngine;
using Firebase.Analytics;
using Pjfb.CampaignBanner;
using Pjfb.Extensions;

namespace Pjfb.Shop
{

    public static class ShopManager
    {
        #region Enum

        public enum PurchaseType
        {
            Price,
            Point,
            External,   // 外部決済
        }

        #endregion
        
        #region SaveData
        private static List<long> confirmedIds;
        private static List<long> confirmedStepUpUnLockEffectIds;
        public static event Action OnPurchaseSuccess = null;
        public static event Action OnPurchaseFail = null;
        
        public static readonly long BanPaymentPenaltyLevel = 999;
        
        public static void SaveConfirmedBillingRewardId(List<BillingRewardBonusDetail> bonusList)
        {
            if (confirmedIds == null) GetConfirmedBillingRewardId();
            var beforeSaveDataCount = confirmedIds.Count;
            foreach (var bonus in bonusList)
            {
                if (confirmedIds.Any(id => id == bonus.mBillingRewardBonusId))
                {
                    // 既に確認ずみならスキップ
                    continue;
                }
                confirmedIds.Add(bonus.mBillingRewardBonusId);
            }
            if (beforeSaveDataCount == confirmedIds.Count)
            {
                return;
            }
            var saveString = string.Empty;
            foreach (var id in confirmedIds)
            {
                if (!string.IsNullOrEmpty(saveString))
                {
                    saveString += ",";
                }
                saveString += id.ToString();
            }
            LocalSaveManager.saveData.shopConfirmedBillingRewardIdString = saveString;
            LocalSaveManager.Instance.SaveData();
        }
        
        public static void SaveConfirmedStepUpUnLockEffectBillingRewardId(BillingRewardBonusDetail bonus)
        {
            if (confirmedStepUpUnLockEffectIds == null) GetConfirmedStepUpUnLockEffectBillingRewardId();
            var beforeSaveDataCount = confirmedStepUpUnLockEffectIds.Count;
            if (confirmedStepUpUnLockEffectIds.Any(id => id == bonus.mBillingRewardBonusId))
            {
                // 既に確認ずみならスキップ
                return;
            }
            confirmedStepUpUnLockEffectIds.Add(bonus.mBillingRewardBonusId);
            if (beforeSaveDataCount == confirmedStepUpUnLockEffectIds.Count)
            {
                return;
            }
            var saveString = string.Empty;
            foreach (var id in confirmedStepUpUnLockEffectIds)
            {
                if (!string.IsNullOrEmpty(saveString))
                {
                    saveString += ",";
                }
                saveString += id.ToString();
            }
            LocalSaveManager.saveData.shopConfirmedStepUpUnLockEffectBillingRewardIdString = saveString;
            LocalSaveManager.Instance.SaveData();
        }

        public static bool HasNewBillingReward(BillingRewardBonusDetail bonus)
        {
            if (confirmedIds == null) GetConfirmedBillingRewardId();
            return !confirmedIds?.Any(id => id == bonus.mBillingRewardBonusId) ?? false;
        }
        
        public static bool HasNewBillingReward(List<BillingRewardBonusDetail> bonusList)
        {
            foreach (var bonus in bonusList)
            {
                var newId = HasNewBillingReward(bonus) || (bonus.buyLimit >= 0 && bonus.buyLimit > bonus.buyCount &&
                                                           MasterManager.Instance.billingRewardMaster
                                                               .FindData(bonus.mBillingRewardId)
                                                               ?.price == 0);
                if (newId)
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool HasNewBillingReward(long mBillingRewardBonusId)
        {
            if (confirmedIds == null) GetConfirmedBillingRewardId();
            return !confirmedIds?.Any(id => id == mBillingRewardBonusId) ?? false;
        }
        
        public static bool HasNewBillingReward(List<long> bonusIdList)
        {
            foreach (var bonusId in bonusIdList)
            {
                var newId = HasNewBillingReward(bonusId);
                if (newId)
                {
                    return true;
                }
            }
            return false;
        }

        private static void GetConfirmedBillingRewardId()
        {
            if (confirmedIds == null)
            {
                confirmedIds = new List<long>();
                var ids = LocalSaveManager.saveData.shopConfirmedBillingRewardIdString;
                if (!string.IsNullOrEmpty(ids))
                {
                    var arrayIds = ids.Split(",");
                    foreach (var arrayId in arrayIds)
                    {
                        confirmedIds.Add(long.Parse(arrayId));
                    }
                }
            }
        }
        
        public static bool HasStepUpUnLockEffectBillingReward(long mBillingRewardBonusId)
        {
            if (confirmedStepUpUnLockEffectIds == null) GetConfirmedStepUpUnLockEffectBillingRewardId();
            return !confirmedStepUpUnLockEffectIds?.Any(id => id == mBillingRewardBonusId) ?? false;
        }
        
        private static void GetConfirmedStepUpUnLockEffectBillingRewardId()
        {
            if (confirmedStepUpUnLockEffectIds == null)
            {
                confirmedStepUpUnLockEffectIds = new List<long>();
                var ids = LocalSaveManager.saveData.shopConfirmedStepUpUnLockEffectBillingRewardIdString;
                if (!string.IsNullOrEmpty(ids))
                {
                    var arrayIds = ids.Split(",");
                    foreach (var arrayId in arrayIds)
                    {
                        confirmedStepUpUnLockEffectIds.Add(long.Parse(arrayId));
                    }
                }
            }
        }

        public static void ResetStaticVariable()
        {
            confirmedIds = null;
            confirmedStepUpUnLockEffectIds = null;
        }
        
        #endregion
        
        #region webAssetPath
        public static string GetStoreBannerPath(long imageNumber)
        {
            return $"{AppEnvironment.AssetBrowserURL}/shop/storeBanner/store_banner_{imageNumber}.png";
        }
        
        public static string GetPackBannerPath(long imageNumber)
        {
            return $"{AppEnvironment.AssetBrowserURL}/shop/packBanner/pack_banner_{imageNumber}.png";
        }
        
        public static string GetPassImagePath(long imageNumber)
        {
            return $"{AppEnvironment.AssetBrowserURL}/shop/pass/pass_{imageNumber}.png";
        }
        #endregion
        
        #region DateUtils
        public static TimeSpan GetRemainTimeSpan(string endDateString, DateTime? nowDate = null)
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var currentDate = nowDate ?? AppTime.Now;
            var endDate = GetDateTimeByString(endDateString);
            if ( !string.IsNullOrEmpty(endDateString) && endDate == DateTime.MinValue)
            {
                return TimeSpan.Zero;
            }

            if (endDate < currentDate)
            {
                return TimeSpan.Zero;
            }

            return endDate - currentDate;
        }
        
        public static DateTime GetDateTimeByString(string dateTimeString)
        {
            var dateTime = DateTime.MinValue;
            var parseSuccess = DateTime.TryParse(dateTimeString, out dateTime);
            if (!parseSuccess) DateTime.TryParse(dateTimeString, out dateTime);
            return dateTime;
        }
        
        public static string GetShopRemainTimeString(TimeSpan span)
        {
          var remainTimeString = $"<size=24>{StringValueAssetLoader.Instance["shop.buy.remaintime"]} </size>";
            if (span.Days > 0) remainTimeString += $"{span.Days}<size=24>{StringValueAssetLoader.Instance["common.day"]}</size>";
            if (span.Hours > 0) remainTimeString += $"{span.Hours}<size=24>{StringValueAssetLoader.Instance["common.hours"]}</size>";
            if (span.Minutes >= 0) remainTimeString += $"{span.Minutes}<size=24>{StringValueAssetLoader.Instance["common.minutes"]}</size>";
            return remainTimeString;
        }

        /// <summary>
        /// 期限がきれていないか
        /// </summary>
        public static bool IsInRemainTime(string endDateString, DateTime? nowDate = null)
        {
            return GetRemainTimeSpan(endDateString, nowDate) > TimeSpan.Zero;
        }
        
        public static bool IsAvailableDateTime(string startDateString, string endDateString, DateTime? nowDate = null)
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var currentDate = nowDate ?? AppTime.Now;
            var startDate = GetDateTimeByString(startDateString);
            var endDate = GetDateTimeByString(endDateString);
            if (!string.IsNullOrEmpty(startDateString) && startDate == DateTime.MinValue)
            {
                return false;
            }
        
            if (!string.IsNullOrEmpty(endDateString) && endDate == DateTime.MinValue)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(startDateString) && startDate > currentDate)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(endDateString) && endDate < currentDate)
            {
                return false;
            }

            return true;
        }
        
        #endregion
        
        #region Purchase
        public static async UniTask OnClickPurchaseButton(BillingRewardBonusDetail detail, PurchaseType purchaseType)
        {
            var billingReward = MasterManager.Instance.billingRewardMaster.FindData(detail.mBillingRewardId);
            var reasonString = string.Empty;
            switch (purchaseType)
            {
                case PurchaseType.Price:
                    if (billingReward.price > 0)
                    {
                        if (!IAPController.Instance.CanMakePayments)
                        {
                            reasonString = StringValueAssetLoader.Instance["shop.error.payments"];
                        } 
                        else if (!IAPController.Instance.IsAvailableProduct(billingReward.appProductId.ToString()))
                        {
                            reasonString = StringValueAssetLoader.Instance["shop.error.product"];
                        }
                        else if (IAPController.Instance.ValidateDeferredPaymentProduct())
                        {
                            reasonString = StringValueAssetLoader.Instance["shop.error.deferred.payment"];
                        }

                        var noPending = CheckAndConfirmPendingProduct();
                        if (!noPending)
                        {
                            return;
                        }
                        
                        if (!string.IsNullOrEmpty(reasonString))
                        {
                            ShowPurchaseErrorModal(reasonString);
                            return;
                        }
                        
                        var isSuccess = await SelectBillingReward(detail);

                        if (!isSuccess)
                        {
                            return;
                        }

                        // 購入処理を行う際にタッチガードとLoadingを出す
                        AppManager.Instance.UIManager.System.TouchGuard.Show();
                        AppManager.Instance.UIManager.System.Loading.Show();

                        IAPController.Instance.PurchaseConsumable(
                            billingReward.appProductId.ToString(),
                            product =>
                            {
                                BuyBillingReward(product, billingReward.GetProductId()).Forget();
                            },
                            (product, reason) =>
                            {
                                // モーダル表示する前にタッチガードとLoadingを消す
                                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                                AppManager.Instance.UIManager.System.Loading.Hide();
                                ShowPurchaseErrorModal(product, reason);
                            });
                    }
                    else
                    {
                        await BuyFreeBillingReward(detail);
                    }
                    break;
                case PurchaseType.Point:
                    var mBillingRewardAlternativePoint =
                        MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                            detail.mBillingRewardId);
                    await ExchangeBillingReward(detail, mBillingRewardAlternativePoint.id, purchaseType);
                    break;
                case PurchaseType.External:
                    if (billingReward.price > 0)
                    {
                        BillingRewardAlternativePointMasterObject billingRewardExternalAlternative = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(billingReward.id, PointMasterObject.PointType.ExternalPoint);
                        await ExchangeBillingReward(detail, billingRewardExternalAlternative.id, purchaseType);
                    }
                    else
                    {
                        await BuyFreeBillingReward(detail);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(purchaseType), purchaseType, null);
            }
            
        }

        public static bool CheckAndShowHasRegisteredBirthday(BillingRewardBonusDetail detail, PurchaseType purchaseType, Action onUpdateUi)
        {
            var billingReward = MasterManager.Instance.billingRewardMaster.FindData(detail.mBillingRewardId);
            if (UserDataManager.Instance.user.hasRegisteredBirthday || billingReward.price <= 0 || purchaseType == PurchaseType.Point) return true;
            ShopAgeCheckModal.Open(new ShopAgeCheckModal.Data(onUpdateUi));
            return false;
        }
        
        public static async UniTask<bool> CheckAndShowHasParentalConsent(BillingRewardBonusDetail detail, PurchaseType purchaseType, Action onUpdateUi)
        {
            var billingReward = MasterManager.Instance.billingRewardMaster.FindData(detail.mBillingRewardId);
            if (UserDataManager.Instance.user.hasParentalConsent || billingReward.price <= 0 || purchaseType == PurchaseType.Point) return true;
            // 保護者の同意を得ているかの確認 Api通信を行うためAwaitしてまつ
            var hasParentalConsent = await ShowParentalConsentConfirmModal();
            // Apiエラーの場合はそのままreturnする
            if (!isSuccessShopUpdateParentalConsentApi) return false;
            // いいえを押した際にtrueになるたfalseで初期化する
            isSuccessShopUpdateParentalConsentApi = false;
            if (hasParentalConsent) return true;
            // 保護者の同意を得ていない場合確認モーダルを出す
            ShowParentalConsentNonConfirmModal(onUpdateUi);
            return false;
        }

        private static async UniTask BuyFreeBillingReward(BillingRewardBonusDetail detail)
        {
            var request = new ShopBuyFreeBillingRewardAPIRequest();
            var post = new ShopBuyFreeBillingRewardAPIPost();
            post.mBillingRewardId = detail.mBillingRewardId;
            post.mBillingRewardBonusId = detail.mBillingRewardBonusId;
            request.SetPostData(post);
            try
            {
                await APIManager.Instance.Connect(request);
                ShowPurchaseSuccessModal();
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                Logger.LogError("ShopBuyFreeBillingRewardAPI error");
            }
        }
        
        private static async UniTask<bool> SelectBillingReward(BillingRewardBonusDetail detail)
        {
            var request = new ShopSelectBillingRewardAPIRequest();
            var post = new ShopSelectBillingRewardAPIPost();
            var success = true;
            post.mBillingRewardId = detail.mBillingRewardId;
            post.mBillingRewardBonusId = detail.mBillingRewardBonusId;
            request.SetPostData(post);
            
            try
            {
                await APIManager.Instance.Connect(request);
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                Logger.LogError("ShopSelectBillingRewardAPI error");
                success = false;
            }
            return success;
        }
        
        private static async UniTask BuyBillingReward(Product product,string productKey, string refreshReceipt = "")
        {
            var request = new ShopBuyBillingRewardAPIRequest();
            var post = new ShopBuyBillingRewardAPIPost();
            post.receipt = refreshReceipt == "" ? product.receipt : refreshReceipt;
            post.productKey = productKey;
            request.SetPostData(post);
            try
            {
                await APIManager.Instance.Connect(request);
                if (IAPController.Instance.ConfirmPendingPurchase(product))
                {
                    var response = request.GetResponseData();
                    UserDataManager.Instance.user.UpdateMonthPayment(response.monthPayment);
                    var isPaid = UserDataManager.Instance.user.isPaid;
                    if (!isPaid)
                    {
                        UserDataManager.Instance.user.UpdateIsPaidValue(true);
                        AdjustManager.TrackEvent(AdjustManager.TrackEventType.FirstPurchase);
                        if (AppInitializer.IsInitialized) 
                        {
                            long price = 0;
                            var billingReward = MasterManager.Instance.billingRewardMaster.values.FirstOrDefault(v => v.appleProductKey == productKey);
                            if (billingReward != null && billingReward.price > 0)
                            {
                                price = billingReward.price;
                            }
                            string eventName = "first_purchase";
                            FirebaseAnalytics.LogEvent(
                                eventName,
                                new Parameter("purchase_user_id", UserData.UserDataManager.Instance.user.uMasterId.ToString()),
                                new Parameter("purchase_price", price.ToString()),
                                new Parameter("product_key", productKey)
                            );
                        }
                    }
                    
                    // モーダル表示する前にタッチガードとLoadingを消す
                    AppManager.Instance.UIManager.System.TouchGuard.Hide();
                    AppManager.Instance.UIManager.System.Loading.Hide();
                    ShowPurchaseSuccessModal();
                }
                else
                {
                    // モーダル表示する前にタッチガードとLoadingを消す
                    AppManager.Instance.UIManager.System.TouchGuard.Hide();
                    AppManager.Instance.UIManager.System.Loading.Hide();
                    ShowPurchaseErrorModal(product,PurchaseFailureReason.Unknown);
                }
            }
            catch (APIException e)
            {
                // レシートのin_appが空の場合、1度だけレシート更新
                if (e.errorParam == (long)APIResponseCode.InAppEmpty && 
                    refreshReceipt == "")
                {
                    // レシートの更新
                    IAPController.Instance.RefreshAppReceipt((receipt)=> 
                    {
                        var pendingProduct = IAPController.Instance.GetFirstPendingProduct();
                        if (pendingProduct != null && IAPController.Instance.ValidatePurchase(pendingProduct))
                        {
                            BuyBillingReward(pendingProduct, pendingProduct.definition.storeSpecificId, pendingProduct.receipt).Forget();
                        }
                    },
                    ()=>{
                        // レシート更新失敗時はタッチガードとLoadingを消す
                        AppManager.Instance.UIManager.System.TouchGuard.Hide();
                        AppManager.Instance.UIManager.System.Loading.Hide();
                    });
                }
                else 
                {
                    // Apiエラーだった場合はタッチガードとLoadingを消す
                    AppManager.Instance.UIManager.System.TouchGuard.Hide();
                    AppManager.Instance.UIManager.System.Loading.Hide();
                    // Apiエラーだった場合はログを出して後の処理を行わない
                    Logger.LogError("ShopBuyBillingRewardAPI error");
                }
            }
        }
        
        private static async UniTask ExchangeBillingReward(BillingRewardBonusDetail detail, long mBillingRewardAlternativePointId, PurchaseType purchaseType)
        {
            var request = new ShopExchangeBillingRewardAPIRequest();
            var post = new ShopExchangeBillingRewardAPIPost();
            post.mBillingRewardId = detail.mBillingRewardId;
            post.mBillingRewardBonusId = detail.mBillingRewardBonusId;
            post.mBillingRewardAlternativePointId = mBillingRewardAlternativePointId;
            request.SetPostData(post);
            try
            {
                await APIManager.Instance.Connect(request);
                if (purchaseType == PurchaseType.Point)
                {
                    ShowExchangeSuccessModal();
                }
                else
                {
                    ShowPurchaseSuccessModal();
                }
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                Logger.LogError("ShopExchangeBillingRewardAPI error");
            }
        }

        private static void ShowPurchaseErrorModal(Product product, PurchaseFailureReason reason)
        {
            var reasonString = string.Empty;
            if (IAPController.Instance.ValidateDeferredPaymentProduct(product))
            {
                reasonString = StringValueAssetLoader.Instance["shop.Deferred.Payment"];
            }
            else
            {
                switch (reason)
                {
                    case PurchaseFailureReason.UserCancelled:
                        reasonString = StringValueAssetLoader.Instance["shop.purchase_error_body"];
                        break;
                    case PurchaseFailureReason.ProductUnavailable:
                    case PurchaseFailureReason.PurchasingUnavailable:
                    case PurchaseFailureReason.PaymentDeclined:
                        reasonString = StringValueAssetLoader.Instance["shop.error.inquiry"];
                        break;

                    case PurchaseFailureReason.DuplicateTransaction:
                    case PurchaseFailureReason.ExistingPurchasePending:
                        reasonString = StringValueAssetLoader.Instance["shop.error.login"];
                        break;
                    case PurchaseFailureReason.Unknown:
                    case PurchaseFailureReason.SignatureInvalid:
                    default:
                        reasonString = StringValueAssetLoader.Instance["shop.error.unknown"];
                        break;
                }
            }
            ShowPurchaseErrorModal(reasonString);
        }
        
        private static void ShowPurchaseSuccessModal()
        {
            var title = StringValueAssetLoader.Instance["shop.purchase_success_title"];
            var body = StringValueAssetLoader.Instance["shop.purchase_success_body"];
            ShowConfirmModal(title,body, async modalWindow =>
            {
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                await modalWindow.CloseAsync();
                OnPurchaseSuccess?.Invoke();
            });
        }
        
        private static void ShowExchangeSuccessModal()
        {
            var title = StringValueAssetLoader.Instance["shop.exchange.confirm_title"];
            var body = StringValueAssetLoader.Instance["shop.exchange.exchange_success"];
            ShowConfirmModal(title,body, async modalWindow =>
            {
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                await modalWindow.CloseAsync();
                OnPurchaseSuccess?.Invoke();
            });
        }
        
        private static void ShowPurchaseErrorModal(string errorBody)
        {
            var title = StringValueAssetLoader.Instance["shop.purchase_error_title"];
            ShowConfirmModal(title,errorBody, async window =>
            {
                await window.CloseAsync();
                OnPurchaseFail?.Invoke();
            });
        }

        private static void ShowConfirmModal(string title, string body, Action<ModalWindow> closeAction)
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                title, body,
                string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], closeAction)));
        }
        
        
        public static bool CheckAndConfirmPendingProduct()
        {
            var pendingProduct = IAPController.Instance.GetFirstPendingProduct();
            if (pendingProduct != null && IAPController.Instance.ValidatePurchase(pendingProduct))
            {
                IAPController.DebugLogProduct(pendingProduct, "CheckAndConfirm");
                ConfirmPendingProduct(pendingProduct);
                return false;
            }
            return true;
        }

        private static void ConfirmPendingProduct(Product pendingProduct)
        {
            var title = StringValueAssetLoader.Instance["common.confirm"];
            var body = StringValueAssetLoader.Instance["shop.pending"];
            
            ShowConfirmModal(title,body, window =>
            {
                window.Close();
                BuyBillingReward(pendingProduct, pendingProduct.definition.storeSpecificId).Forget();
            });
        }

        /// <summary> ShopUpdateParentalConsentAPIが成功しているかを保持 /// </summary>
        private static bool isSuccessShopUpdateParentalConsentApi;
        
        /// <summary>
        /// 保護者の同意を得ているかの確認モーダルを出す
        /// </summary>
        /// <returns></returns>
        private static async UniTask<bool> ShowParentalConsentConfirmModal()
        {
            var parentalConsentConfirmModal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm,new ConfirmModalData
            (
                StringValueAssetLoader.Instance["shop.parental_consent.title"],
                StringValueAssetLoader.Instance["shop.parental_consent.confirm"],
                string.Empty, 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.yes"], confirmWindow =>
                {
                    //”はい”ボタンの押す処理
                    RequestUpdateParentalConsentAsync(confirmWindow.Close).Forget();
                }),new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.no"], confirmWindow =>
                {
                    //”いいえ”ボタンの押す処理
                    // 同意しない場合Apiを投げないためTrueにしておく
                    isSuccessShopUpdateParentalConsentApi = true;
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                    confirmWindow.Close();
                })
            ));
            await parentalConsentConfirmModal.WaitCloseAsync();
            return UserDataManager.Instance.user.hasParentalConsent;
        }
        
        /// <summary>
        /// 保護者の同意を得ていない場合の確認モーダルを出す
        /// </summary>
        private static void ShowParentalConsentNonConfirmModal(Action onComplete)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm,new ConfirmModalData
            (
                StringValueAssetLoader.Instance["shop.parental_consent.title.non_confirm"],
                StringValueAssetLoader.Instance["shop.parental_consent.non_confirm"],
                string.Empty, 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], confirmWindow =>
                {
                    //”いいえ”ボタンの押す処理
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                    confirmWindow.Close();
                    onComplete?.Invoke();
                })
            ));
        }

        /// <summary>
        /// 保護者の同意を得ている場合に投げるApi
        /// </summary>
        /// <param name="onComplete"></param>
        private static async UniTask RequestUpdateParentalConsentAsync(Action onComplete)
        {
            try
            {
                var request = new ShopUpdateParentalConsentAPIRequest();
                var post = new ShopUpdateParentalConsentAPIPost();
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                var response = request.GetResponseData();
                UserDataManager.Instance.user.UpdateParentalConsent(response.hasParentalConsent);
                onComplete?.Invoke();
                isSuccessShopUpdateParentalConsentApi = true;
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                Logger.LogError("ShopUpdateParentalConsentAPI error");
                isSuccessShopUpdateParentalConsentApi = false;
            }
        }
        
        #endregion

        #region SaleIntroduction

        public static List<NativeApiSaleIntroduction> ShowSaleIntroductionList { get; } = new();

        public static List<NativeApiSaleIntroduction> SaleIntroductionList { get; } = new();

        // 再表示時のキャッシュ
        private static List<NativeApiSaleIntroduction> lastTriggeredSaleIntroductions;

        public static void UpdateSaleIntroduction(NativeApiSaleIntroduction[] saleIntroductionList)
        {
            if(saleIntroductionList == null) return;
            
            // 先に期限切れのセール情報を削除
            SaleIntroductionList.RemoveAll(sale => sale.expireAt.TryConvertToDateTime().IsPast(AppTime.Now));
            foreach (var saleIntroduction in saleIntroductionList)
            {
                // リマインドの場合は既存のものを削除して追加
                if (saleIntroduction.isRemind == true)
                {
                    SaleIntroductionList.RemoveAll(data => data.mSaleIntroductionId == saleIntroduction.mSaleIntroductionId && data.expireAt == saleIntroduction.expireAt);
                }
                else
                {
                    // 同一ID、同一期限が既に存在している場合continue
                    if(SaleIntroductionList.Any(data => data.mSaleIntroductionId == saleIntroduction.mSaleIntroductionId && data.expireAt == saleIntroduction.expireAt)) continue;
                }
                SaleIntroductionList.Add(saleIntroduction);
            }
        }
        
        public static void UpdateShowSaleIntroduction(NativeApiSaleIntroduction[] saleIntroductionList)
        {
            if(saleIntroductionList == null) return;
            
            // 先に期限切れのセール情報を削除
            ShowSaleIntroductionList.RemoveAll(sale => sale.expireAt.TryConvertToDateTime().IsPast(AppTime.Now));
            foreach (var saleIntroduction in saleIntroductionList)
            {
                // 同一ID、同一期限が既に存在している場合continue
                if(ShowSaleIntroductionList.Any(data => data.mSaleIntroductionId == saleIntroduction.mSaleIntroductionId && data.expireAt == saleIntroduction.expireAt)) continue;
                ShowSaleIntroductionList.Add(saleIntroduction);
            }
        }
        
        /// <summary> 特定タイミングでのセール情報の表示 /// </summary>
        public static void TryShowSaleIntroduction(SaleIntroductionDisplayType type, Action onFinish = null)
        {
            // リマインドを含むかどうか
            bool isRemindTiming = type == SaleIntroductionDisplayType.HomeTop;
            
            // 該当のページで発生したセール情報を優先表示するリスト
            List<NativeApiSaleIntroduction> triggeredList = new ();
            foreach (NativeApiSaleIntroduction saleIntroduction in ShowSaleIntroductionList)
            {
                SaleIntroductionMasterObject mSaleIntroduction = MasterManager.Instance.saleIntroductionMaster.FindData(saleIntroduction.mSaleIntroductionId);
                
                // リマインドかどうか
                if (saleIntroduction.isRemind)
                {
                    // 指定位置でのみ追加
                    if (isRemindTiming)
                    {
                        triggeredList.Add(saleIntroduction);
                    }
                }
                else
                {
                    // 表示位置が一致
                    if (mSaleIntroduction.DisplayPageType == type)
                    {
                        triggeredList.Add(saleIntroduction);
                    }
                }
            }
            
            if (triggeredList.Any())
            {
                // 戻ったときに優先するリストを保持
                lastTriggeredSaleIntroductions = triggeredList;
                SecretBannerModalWindow.OpenModal(false, () => onFinish?.Invoke(), triggeredList).Forget();
                
                // トリガーされたものを一度開いたら既読扱いにする
                ShowSaleIntroductionList.RemoveAll(saleIntroduction => triggeredList.Contains(saleIntroduction));
            }
            else
            {
                onFinish?.Invoke();
            }
        }
        
        /// <summary> セール情報を全て表示 /// </summary>
        public static void ShowAllSaleIntroduction(Action onClose = null)
        {
            // 優先表示用のリストは初期化
            lastTriggeredSaleIntroductions = null;
            SecretBannerModalWindow.OpenModal(true, onClose).Forget();
        }

        public static bool HasSaleIntroduction(NativeApiSaleIntroduction[] saleIntroductionList)
        {
            return saleIntroductionList != null && saleIntroductionList.Length > 0;
        }

        /// <summary>
        /// 現在のセール情報を表示優先度順に取得
        /// </summary>
        public static NativeApiSaleIntroduction[] GetSaleIntroductionsOrderPriority()
        {
            // 現在発生しているものをソートして返す
            return GetSpecifySaleIntroductionsOrderedByPriority();
        }
        
        /// <summary>
        /// リスト内のセール情報を優先度順に並び替えて返す
        /// </summary>
        public static NativeApiSaleIntroduction[] GetSpecifySaleIntroductionsOrderedByPriority(List<NativeApiSaleIntroduction> triggeredSaleIntroductions = null)
        {
            // 発生したセール情報のId判定用のリストを作成
            HashSet<long> triggeredIds = triggeredSaleIntroductions?.Select(s => s.mSaleIntroductionId).ToHashSet();
            
            return SaleIntroductionList.
                // 期間内のセールのみ
                Where(v => IsInRemainTime(v.expireAt) == true).
                // 優先度1:そのとき発生したものがあれば優先
                OrderByDescending(v => ContainsTriggeredList(v.mSaleIntroductionId, triggeredIds)).
                // 優先度2:期間が近い順
                ThenBy(v => GetDateTimeByString(v.expireAt)).
                // 優先度3:id順
                ThenBy(v => v.mSaleIntroductionId).ToArray();
        }

        /// <summary>
        /// 発生したリスト内にIdが含まれているか
        /// </summary>
        private static bool ContainsTriggeredList(long id, HashSet<long> triggeredSaleIntroductions)
        {
            if (triggeredSaleIntroductions == null) return false;
            return triggeredSaleIntroductions.Contains(id);
        }
        
        /// <summary>
        /// セール情報の削除
        /// </summary>
        public static void RemoveSaleIntroduction(long saleIntroductionId)
        {
            NativeApiSaleIntroduction removeData = SaleIntroductionList.FirstOrDefault(data => data.mSaleIntroductionId == saleIntroductionId);
            SaleIntroductionList.Remove(removeData);
        }

        public static void ClearCache()
        {
            ShowSaleIntroductionList?.Clear();
            SaleIntroductionList?.Clear();
            lastTriggeredSaleIntroductions = null;
        }
        
        /// <summary>
        /// シークレットバナーを開く
        /// </summary>
        private static void OpenSecretBannerModal()
        {
            // 直近トリガーされたものがあれば優先するリストも渡す
            SecretBannerModalWindow.OpenModal(false, null, lastTriggeredSaleIntroductions).Forget();
        }
        
        /// <summary>
        /// 購入後にシークレットバナーを開きなおす処理の解除
        /// </summary>
        public static void UnregistrationOpenSecretBannerModal()
        {
            OnPurchaseSuccess -= OpenSecretBannerModal;
            OnPurchaseFail -= OpenSecretBannerModal;
        }
        
        /// <summary>
        /// 購入後にシークレットバナーを開きなおす処理の登録
        /// </summary>
        public static void RegistrationOpenSecretBannerModal()
        {
            UnregistrationOpenSecretBannerModal();
            // 購入成功時
            if (OnPurchaseSuccess == null || OnPurchaseSuccess.GetInvocationList().Length <= 0)
            {
                OnPurchaseSuccess += OpenSecretBannerModal;
            }
            
            // 購入失敗時
            if (OnPurchaseFail == null || OnPurchaseFail.GetInvocationList().Length <= 0)
            {
                OnPurchaseFail += OpenSecretBannerModal;
            }
        }

        // 外部通貨を所持しているかどうか
        public static bool HasExternalPoint(List<BillingRewardBonusDetail> list )
        {
            if(list == null) return false;
            foreach (BillingRewardBonusDetail billingRewardBonus in list)
            {
                if (billingRewardBonus.mBillingRewardBonusId == default) continue;
                BillingRewardMasterObject billingReward = MasterManager.Instance.billingRewardMaster.FindData(billingRewardBonus.mBillingRewardId);
                if(billingReward == null) continue;
                BillingRewardAlternativePointMasterObject externalRewardPoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(billingReward.id, PointMasterObject.PointType.ExternalPoint);
                if(externalRewardPoint == null) continue;
                long pointValue = UserDataManager.Instance.GetExpiryPointValue(externalRewardPoint.mPointId);
                return pointValue > 0;
            }
            return false;
        }
        
        // 購入方法の切り替え処理
        public static PurchaseType ChangePurchase(PurchaseType purchaseType, List<BillingRewardBonusDetail> dataList)
        {
            purchaseType = purchaseType switch
            {
                PurchaseType.Price => UserDataManager.Instance.IsCompanyUser() ? PurchaseType.Point : PurchaseType.External,
                PurchaseType.Point => HasExternalPoint(dataList) ?PurchaseType.External : PurchaseType.Price,
                PurchaseType.External => PurchaseType.Price,
                _ => throw new ArgumentOutOfRangeException()
            };
            LocalSaveManager.saveData.shopPurchaseType = purchaseType;
            LocalSaveManager.saveData.shopPurchaseTypeChangeFlag = true;
            LocalSaveManager.Instance.SaveData();
            return purchaseType;
        }

        // 購入方法切り替えボタン表示判定
        public static bool CheckPurchaseChangeFlg()
        {
            // 購入方法の切り替えフラグが立っているか
            if(LocalSaveManager.saveData.shopPurchaseTypeChangeFlag)
            {
                return true;
            }

            // 社内ユーザーか
            if (UserDataManager.Instance.IsCompanyUser())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 購入前に年齢確認と保護者の同意確認を行う
        /// 確認済みの場合はtrueを返す
        /// </summary>
        public static async UniTask<bool> CheckAndShowBeforePurchaseConfirm(BillingRewardBonusDetail bonus, PurchaseType purchaseType, Action onUpdateUi = null)
        {
            // 年齢設定がされているか(無料パック,交換は除く)
            if (!CheckAndShowHasRegisteredBirthday(bonus, purchaseType, onUpdateUi)) return false;

            // 保護者の同意がされているか(無料パック,交換は除く)
            var hasParentalConsent = await CheckAndShowHasParentalConsent(bonus, purchaseType, onUpdateUi);
            if (!hasParentalConsent) return false;
            return true;
        }
        
        public static async UniTask OpenPurchaseConfirm(BillingRewardBonusDetail bonus, PurchaseType purchaseType, Action onUpdateUi = null)
        {
            // 年齢確認
            if(await CheckAndShowBeforePurchaseConfirm(bonus, purchaseType, onUpdateUi) == false) return;

            // 年齢設定と保護者の同意がされている場合（無料パックの場合は除く）最終確認モーダルを開く
            ShopProductPurchaseModal.Open(new ShopProductPurchaseModal.Data(bonus, purchaseType, onUpdateUi));
        }
        
        public static async UniTask<ShopGetBillingRewardBonusListAPIResponse> GetShopGetBillingRewardBonusListAPI()
        {
            ShopGetBillingRewardBonusListAPIRequest request = new ShopGetBillingRewardBonusListAPIRequest();
            await APIManager.Instance.Connect(request); 
            return request.GetResponseData();
        }
        
        ///<summary>外部決済サイトへ遷移する</summary>
        public static void OpenExternalPurchaseSite()
        {
            Application.OpenURL("https://bluelock-pwc.com/purchase_shop_coin/");
        }
        
        #endregion
    }
}