using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CruFramework.Page;
using Pjfb.Networking.App.Request;
using UnityEngine;

using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine.UI;
using Pjfb.UI;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Shop
{

    public class ShopPassItem : ListItemBase
    {
        public enum PassType
        {
            BattlePass,
            LoginPass,
        }

        public class ItemParams : ItemParamsBase
        {
            public BillingRewardBonusDetail bonus;
            public bool isNew;
            public bool isBan;
            public ShopManager.PurchaseType PurchaseType;
            public Action OnUpdateUi;
            public Action<long> OnSelectedBillingRewardBonus;
        }
        
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private RectTransform BattlePassItem;
        [SerializeField]
        private RectTransform LoginPassItem;
        
        [Header("バトルパスUi")]
        [SerializeField] private TMP_Text battlePassNameText;
        [SerializeField] private TMP_Text battlePassPriceText;
        [SerializeField] private GameObject battlePassRequiredPointRoot;
        [SerializeField] private IconImage battlePassRequiredPointImage;
        [SerializeField] private TMP_Text battlePassRequiredPointValueText;
        [SerializeField] private GameObject battlePassScheduleBaseRoot;
        [SerializeField] private TMP_Text battlePassSpecialText;
        [SerializeField] private GameObject battlePassSpecialRoot;
        [SerializeField] private TMP_Text battlePassScheduleText;
        [SerializeField] private GameObject battlePassScheduleRoot;
        [SerializeField] private GameObject battlePassNewBadge;
        [SerializeField] private GameObject battlePassCoverObject;
        [SerializeField] private TMP_Text battlePassCoverText;
        [SerializeField] private UIButton battlePassBuyButton;
        [SerializeField] private TMP_Text battlePassBuyButtonText;
        [SerializeField] private GameObject battlePassSoldOutRoot;
        [SerializeField] private GameObject battlePassScheduleSpecialCoverRoot;
        [SerializeField] private IconImage battlePassExternalPointImage;
        [SerializeField] private TMP_Text battlePassExternalPointText;

        [Header("ログインパスUi")]
        [SerializeField] private TMP_Text loginPassNameText;
        [SerializeField] private TMP_Text loginPassPriceText;
        [SerializeField] private GameObject loginPassRequiredPointRoot;
        [SerializeField] private IconImage loginPassRequiredPointImage;
        [SerializeField] private TMP_Text loginPassRequiredPointValueText;
        [SerializeField] private GameObject loginPassScheduleBaseRoot;
        [SerializeField] private TMP_Text loginPassSpecialText;
        [SerializeField] private GameObject loginPassSpecialRoot;
        [SerializeField] private TMP_Text loginPassScheduleText;
        [SerializeField] private GameObject loginPassScheduleRoot;
        [SerializeField] private GameObject loginPassNewBadge;
        [SerializeField] private GameObject loginPassCoverObject;
        [SerializeField] private TMP_Text loginPassCoverText;
        [SerializeField] private UIButton loginPassBuyButton;
        [SerializeField] private TMP_Text loginPassBuyButtonText;
        [SerializeField] private CancellableWebTexture loginPassWebTexture;
        [SerializeField] private UIButton loginPassInfoButton;
        [SerializeField] private GameObject loginPassSoldOutRoot;
        [SerializeField] private GameObject loginPassScheduleSpecialCoverRoot;
        [SerializeField] private IconImage loginPassExternalPointImage;
        [SerializeField] private TMP_Text loginPassExternalPointText;

        private ItemParams itemParams;
        private PassType currentPassType;

        /// <summary>
        /// 全体的な初期化
        /// </summary>
        /// <param name="_itemParams"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Init(ItemParamsBase _itemParams)
        {
            itemParams = (ItemParams) _itemParams;
            currentPassType = ConvertCategoryToPassType(itemParams.bonus.GetCategory());
            BattlePassItem?.gameObject.SetActive(currentPassType == PassType.BattlePass);
            LoginPassItem?.gameObject.SetActive(currentPassType == PassType.LoginPass);

            var rectTransformHeight = currentPassType == PassType.BattlePass ? BattlePassItem.rect.height : LoginPassItem.rect.height;
            rectTransform.sizeDelta = new Vector2 (rectTransform.rect.width, rectTransformHeight);
            
            // ログインパス or バトルパスのUI反映
            switch (currentPassType)
            {
                case PassType.BattlePass:
                    SetBattlePassUi();
                    break;
                case PassType.LoginPass:
                    SetLoginPassUi();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// バトルパスのUI反映を行う
        /// </summary>
        private void SetBattlePassUi()
        {
            //Todo: バトルパス対応時に対応
        }

        /// <summary>
        /// ログインパスのUI反映を行う
        /// </summary>
        private void SetLoginPassUi()
        {
            var mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(itemParams.bonus.mBillingRewardId);
            if (mBillingReward == null)
            {
                CruFramework.Logger.Log($"mBillingRewardがnulです。mBillingRewardBonusId : {itemParams.bonus.mBillingRewardBonusId}");
                return;
            }
            loginPassNameText.text = itemParams.bonus.name;
            loginPassPriceText.gameObject.SetActive(itemParams.PurchaseType == ShopManager.PurchaseType.Price);
            loginPassRequiredPointRoot.SetActive(itemParams.PurchaseType == ShopManager.PurchaseType.Point);
            loginPassExternalPointText.gameObject.SetActive(itemParams.PurchaseType == ShopManager.PurchaseType.External);
            var sb = new StringBuilder();
            switch (itemParams.PurchaseType)
            {
                case ShopManager.PurchaseType.Price:
                    loginPassPriceText.text = sb.AppendFormat(StringValueAssetLoader.Instance["shop.price"], mBillingReward.price.GetStringNumberWithComma()).ToString();
                    loginPassBuyButtonText.text = StringValueAssetLoader.Instance["shop.purchase_button"];
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
                    loginPassRequiredPointImage.SetTexture(mBillingRewardAlternativePoint.mPointId);
                    loginPassRequiredPointValueText.text = mBillingRewardAlternativePoint.value.ToString();
                    loginPassBuyButtonText.text = StringValueAssetLoader.Instance["shop.exchange.button"];
                    break;
                case ShopManager.PurchaseType.External:
                    BillingRewardAlternativePointMasterObject mBillingRewardAlternativeExternal = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(mBillingReward.id, PointMasterObject.PointType.ExternalPoint);
                    if(mBillingRewardAlternativeExternal == null) break;
                    loginPassExternalPointImage.SetTexture(mBillingRewardAlternativeExternal.mPointId);
                    loginPassExternalPointText.text = mBillingRewardAlternativeExternal.value.GetStringNumberWithComma();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SetPassImage(itemParams.bonus.imageNumber).Forget();

            var isExistSpecialRoot = !string.IsNullOrEmpty(itemParams.bonus.appealText);
            loginPassSpecialRoot.SetActive(isExistSpecialRoot);
            loginPassSpecialText.text = itemParams.bonus.appealText;
            var closedDateTime = itemParams.bonus.closedDatetime;
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var remainTime = ShopManager.GetRemainTimeSpan(closedDateTime, AppTime.Now);
            var isExistScheduleRoot = !string.IsNullOrEmpty(closedDateTime) && remainTime.Days <= 365;
            loginPassScheduleRoot.SetActive(isExistScheduleRoot);
            loginPassScheduleText.text = ShopManager.GetShopRemainTimeString(remainTime);
            loginPassScheduleBaseRoot.SetActive(isExistSpecialRoot || isExistScheduleRoot);

            loginPassNewBadge.SetActive(itemParams.isNew);
            var mLonginPass = MasterManager.Instance.loginPassMaster.FindDataByMBillingRewardBonusId(itemParams.bonus.mBillingRewardBonusId);
            if (mLonginPass == null)
            {
                CruFramework.Logger.Log($"mLonginPassがnulです。mBillingRewardBonusId : {itemParams.bonus.mBillingRewardBonusId}");
                return;
            }
            var uTag = UserDataManager.Instance.tag?.FirstOrDefault(tempTag => tempTag == mLonginPass.adminTagId);
            var hasPossessionPass = uTag != null && uTag > 0;
            loginPassBuyButton.interactable = itemParams.PurchaseType switch
            {
                ShopManager.PurchaseType.Price =>
                    // ユーザーが所持していない場合は購入ボタンをアクティブにする
                    !hasPossessionPass,
                ShopManager.PurchaseType.Point =>
                    // ユーザーが所持していない場合かつ交換できる場合は購入ボタンをアクティブにする
                    !hasPossessionPass && CanPointExchange(),
                ShopManager.PurchaseType.External =>
                    // ユーザーが所持していないかつ外部決済アイテムで購入できる場合はボタンをアクティブにする
                    !hasPossessionPass && CanExternalPointExchange(mBillingReward),
                _ => throw new ArgumentOutOfRangeException()
            };
            loginPassCoverObject.SetActive(false);
            // お知らせのURLがない場合は詳細ボタンを非表示にする
            loginPassInfoButton.gameObject.SetActive(!string.IsNullOrEmpty(itemParams.bonus.detailUrl));

            // 購入済みの場合は売り切れ表示を行う
            loginPassSoldOutRoot.SetActive(uTag != null && uTag > 0);
            // 購入済みの場合はScheduleとSpecialのグレーアウト表示を行う
            loginPassScheduleSpecialCoverRoot.SetActive(uTag != null && uTag > 0);

            if(loginPassSoldOutRoot.activeSelf) return;
            
            // 返金で課金制限がかかっていた場合の制限表示
            if (itemParams.isBan)
            {
                loginPassCoverObject.SetActive(true);
                loginPassCoverText.text = StringValueAssetLoader.Instance["shop.purchase.limit_ban"];
                loginPassScheduleSpecialCoverRoot.SetActive(true);
                return;
            }

            // 購入制限がかかっている場合は購入制限をかける
            if (itemParams.PurchaseType != ShopManager.PurchaseType.External)
            {
                if (!loginPassCoverObject.activeSelf && UserDataManager.Instance.user.hasRegisteredBirthday)
                {
                    var monthPayment = UserDataManager.Instance.user.monthPayment;
                    var monthPaymentLimit = UserDataManager.Instance.user.monthPaymentLimit;
                    if (mBillingReward.price + monthPayment > monthPaymentLimit)
                    {
                        loginPassCoverObject.SetActive(true);
                        loginPassCoverText.text = StringValueAssetLoader.Instance["shop.purchase.limit"];
                        loginPassScheduleSpecialCoverRoot.SetActive(true);
                    }
                }
            }
        }
        
        /// <summary>
        /// Image画像を設定する
        /// </summary>
        /// <param name="imageNumber"></param>
        private async UniTask SetPassImage(long imageNumber)
        {
            //Todo: バトルパスは別途対応が必要
            var key = ShopManager.GetPassImagePath(imageNumber);
            await loginPassWebTexture.SetTextureAsync(key);
        }

        /// <summary>
        /// 購入ボタンを押した際の処理
        /// </summary>
        public void OnClickPurchaseButton()
        {
            ShopManager.OpenPurchaseConfirm(itemParams.bonus, itemParams.PurchaseType, itemParams.OnUpdateUi).Forget();
            itemParams.OnSelectedBillingRewardBonus?.Invoke(itemParams.bonus.mBillingRewardBonusId);
        }
        
        /// <summary>
        /// 詳細ボタンを押した際の処理
        /// </summary>
        public void OnClickInfoButton()
        {
            News.NewsManager.TryShowNews(isClickNewsButton: true, isFromTitle: false, newsArticleForcedDisplayData: null, onComplete: null, openArticleUrl: itemParams.bonus.detailUrl);
        }

        /// <summary>
        /// 現在のカテゴリーを取得
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private PassType ConvertCategoryToPassType(BillingRewardBonusCategory category)
        {
            return category == BillingRewardBonusCategory.Pass ? PassType.LoginPass : PassType.BattlePass;
        }
        
        private bool CanPointExchange()
        {
            var mBillingRewardAlternativePoint =
                MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(
                    itemParams.bonus.mBillingRewardId);
            if (mBillingRewardAlternativePoint == null)
            {
                CruFramework.Logger.LogError($"mBillingRewardAlternativePointが取得できませんでした。　mBillingRewardId:{itemParams.bonus.mBillingRewardId}");
                return false;
            }
            var point = UserDataManager.Instance.point.Find(mBillingRewardAlternativePoint.mPointId);
            return (point?.value ?? 0) >= mBillingRewardAlternativePoint.value;
        }
        
        /// <summary>
        /// 外部決済アイテムで購入できるかどうか
        /// </summary>
        /// <param name="billingMasterObject"></param>
        /// <returns></returns>
        private bool CanExternalPointExchange(BillingRewardMasterObject billingMasterObject)
        {
            BillingRewardAlternativePointMasterObject mBillingRewardAlternativePoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(billingMasterObject.id, PointMasterObject.PointType.ExternalPoint);
            if (mBillingRewardAlternativePoint == null) return false;
            // 期限内のアイテム数で判断
            long pointValue = UserDataManager.Instance.GetExpiryPointValue(mBillingRewardAlternativePoint.mPointId);
            return pointValue >= mBillingRewardAlternativePoint.value;
        }
    }
}