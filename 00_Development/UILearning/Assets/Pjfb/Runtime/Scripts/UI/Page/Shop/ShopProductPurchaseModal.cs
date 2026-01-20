using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;
using TMPro;

namespace Pjfb
{
    public class ShopProductPurchaseModal : ModalWindow
    {
        public class Data
        {
            public BillingRewardBonusDetail Bonus;
            public ShopManager.PurchaseType PurchaseType;
            public Action OnUpdateUi = null;
            public bool IsAgeConfirm = false;
            public Action OnClickPurchase = null;
            public bool IsBan = false;

            public Data(BillingRewardBonusDetail bonus, ShopManager.PurchaseType purchaseType, Action onUpdateUi = null, bool isAgeConfirm = false, Action onClickPurchase = null, bool isBan = false)
            {
                Bonus = bonus;
                PurchaseType = purchaseType;
                OnUpdateUi = onUpdateUi;
                IsAgeConfirm = isAgeConfirm;
                OnClickPurchase = onClickPurchase;
                IsBan = isBan;
            }
        }
        
        private enum ParentRectTransformType
        {
            None,
            Product,
            Bonus
        }

        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject productRoot;
        [SerializeField] private RectTransform productRootRectTransform;
        [SerializeField] private GameObject bonusRoot;
        [SerializeField] private RectTransform bonusRootRectTransform;
        [SerializeField] private MerchandiseListItemUi merchandiseListItemUiPrefab;
        [SerializeField] private GameObject freeTextRoot;
        [SerializeField] private TMP_Text freeText;
        [SerializeField] private TMP_Text buyButtonText;
        [SerializeField] private GameObject cover;
        [SerializeField] private GameObject purchaseChangeButtonRoot;
        [SerializeField] private UIButton purchaseButton;

        private Data modalData;
        private List<MerchandiseListItemUi> productMerchandiseListItemUiList = new List<MerchandiseListItemUi>();
        private List<MerchandiseListItemUi> bonusMerchandiseListItemUiList = new List<MerchandiseListItemUi>();
        private List<BillingRewardBonusDetail> billingRewardBonusList = new List<BillingRewardBonusDetail>();

        private bool isInitializedUi = false;

        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopProductPurchase, data);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            billingRewardBonusList = new List<BillingRewardBonusDetail>{modalData.Bonus};
            purchaseChangeButtonRoot.SetActive(ShopManager.HasExternalPoint(billingRewardBonusList) || ShopManager.CheckPurchaseChangeFlg());
            UpdateView();
            isInitializedUi = true;
            return base.OnPreOpen(args, token);
        }

        private void UpdateView()
        {
            productMerchandiseListItemUiList.Clear();
            bonusMerchandiseListItemUiList.Clear();
            cover.SetActive(false);
            purchaseButton.interactable = true;
            
            var detail = modalData.Bonus;
            var mBillingReward = MasterManager.Instance.billingRewardMaster.FindData(detail.mBillingRewardId);
            if (mBillingReward == null)
            {
                CruFramework.Logger.LogError($"mBillingRewardが取得できませんでした。　mBillingRewardId:{detail.mBillingRewardId}");
                return;
            }
            long price = 0;
            BillingRewardAlternativePointMasterObject mBillingRewardAlternativePoint = null;
            BillingRewardAlternativePointMasterObject mBillingRewardExternalPoint = null;

            // 価格
            switch (modalData.PurchaseType)
            {
                case ShopManager.PurchaseType.Price:
                    price = mBillingReward.price;
                    break;
                case ShopManager.PurchaseType.Point:
                    mBillingRewardAlternativePoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(detail.mBillingRewardId);
                    price = mBillingRewardAlternativePoint.value;
                    break;
                case ShopManager.PurchaseType.External:
                    mBillingRewardExternalPoint = MasterManager.Instance.billingRewardAlternativePointMaster.FindDataByMBillingRewardId(mBillingReward.id, PointMasterObject.PointType.ExternalPoint);
                    price = mBillingRewardExternalPoint.value;
                    break;
            }
            var isPaidPack = price > 0;

            //購入ボタンや金額テキストの設定
            var sb = new StringBuilder();
            if (isPaidPack)
            {
                switch(modalData.PurchaseType)
                {
                    case ShopManager.PurchaseType.Price:
                        priceText.text = detail.GetCategory() == BillingRewardBonusCategory.Pass ||
                                         detail.GetCategory() == BillingRewardBonusCategory.BattlePass
                            ? sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.pass"], price.GetStringNumberWithComma()).ToString()
                            : sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.price"], price.GetStringNumberWithComma()).ToString();
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.purchase_button"];
                        // 課金上限に達している場合はグレーアウトする
                        long monthPayment = UserDataManager.Instance.user.monthPayment;
                        long monthPaymentLimit = UserDataManager.Instance.user.monthPaymentLimit;
                        bool isPurchaseRestricted = price + monthPayment > monthPaymentLimit;
                        cover.SetActive(isPurchaseRestricted);
                        purchaseButton.interactable = isPurchaseRestricted == false && modalData.IsBan == false;
                        break;
                    case ShopManager.PurchaseType.Point:

                        if (mBillingRewardAlternativePoint == null)
                        {
                            CruFramework.Logger.LogError($"mBillingRewardAlternativePointが取得できませんでした。　mBillingRewardId:{detail.mBillingRewardId}");
                            break;
                        }

                        var mPoint =
                            MasterManager.Instance.pointMaster.FindData(mBillingRewardAlternativePoint.mPointId);
                        if (mPoint == null)
                        {
                            CruFramework.Logger.LogError($"mPointが取得できませんでした。　mPointId:{mBillingRewardAlternativePoint.mPointId}");
                            break;
                        }
                        
                        priceText.text = detail.GetCategory() == BillingRewardBonusCategory.Pass ||
                                         detail.GetCategory() == BillingRewardBonusCategory.BattlePass
                            ? sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.pass.exchange"], mPoint.name).ToString()
                            : sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.pack.exchange"], mPoint.name).ToString();
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.exchange.button"];
                        break;
                    case ShopManager.PurchaseType.External:
                        PointMasterObject externalPoint = MasterManager.Instance.pointMaster.FindData(mBillingRewardExternalPoint.mPointId);
                        if (detail.GetCategory() == BillingRewardBonusCategory.Pass || detail.GetCategory() == BillingRewardBonusCategory.BattlePass)
                        {
                            priceText.text = sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.external.pass"], mBillingRewardExternalPoint.value.GetStringNumberWithComma(),externalPoint.unitName).ToString();
                        }
                        else
                        {
                            priceText.text = sb.AppendFormat(StringValueAssetLoader.Instance["shop.product.purchase.external.price"], mBillingRewardExternalPoint.value.GetStringNumberWithComma(),externalPoint.unitName).ToString();
                        }
                        buyButtonText.text = StringValueAssetLoader.Instance["shop.purchase_button"];
                        // 所持数が足りていない場合、課金制限がかかっている場合各所グレーアウト
                        bool isCanPurchase = UserDataManager.Instance.GetExpiryPointValue(mBillingRewardExternalPoint.mPointId) >= price && modalData.IsBan == false;
                        purchaseButton.interactable = isCanPurchase;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                priceText.text = StringValueAssetLoader.Instance["shop.product.purchase.free"];
                buyButtonText.text = StringValueAssetLoader.Instance["shop.free.buy"];
            }

            
            // 表示するアイテムの設定(初回のみ更新)
            if (isInitializedUi) return;
            if (isPaidPack)
            {
                foreach (var prize in mBillingReward.prizeJson)
                {
                    var freeValue = prize.args.value - mBillingReward.paidPointValue;
                    var paidValue = mBillingReward.paidPointValue;
                    if (freeValue > 0)
                    {
                        // 有償アイテム
                        if (paidValue > 0)
                        {
                            var paidPrize = new Master.PrizeJsonWrap(prize);
                            paidPrize.args.value = paidValue;
                            var paidListItemUi = Instantiate(merchandiseListItemUiPrefab, productRootRectTransform);
                            paidListItemUi.InitializeUi(paidPrize, true);
                            productMerchandiseListItemUiList.Add(paidListItemUi);
                        }

                        // 無償アイテム
                        var freePrize = new Master.PrizeJsonWrap(prize);
                        freePrize.args.value = freeValue;
                        var parentRectTransformType =
                            GetParentRectTransformType(freePrize.type, freePrize.args.mPointId);
                        // Noneの場合はスキップする
                        if (parentRectTransformType == ParentRectTransformType.None) continue;
                        var freeListItemUi = Instantiate(merchandiseListItemUiPrefab, GetParentListRootRectTransform(parentRectTransformType));
                        freeListItemUi.InitializeUi(freePrize, false);
                        switch (parentRectTransformType)
                        {
                            case ParentRectTransformType.None:
                                break;
                            case ParentRectTransformType.Product:
                                productMerchandiseListItemUiList.Add(freeListItemUi);
                                break;
                            case ParentRectTransformType.Bonus:
                                bonusMerchandiseListItemUiList.Add(freeListItemUi);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        // 有償アイテム
                        var paidListItemUi = Instantiate(merchandiseListItemUiPrefab, productRootRectTransform);
                        paidListItemUi.InitializeUi(prize, true);
                        productMerchandiseListItemUiList.Add(paidListItemUi);
                    }
                }
            }

            foreach (var prize in detail.prizeJsonList)
            {
                var parentRectTransformType =
                    GetParentRectTransformType(prize.type, prize.args.mPointId);
                // Noneの場合はスキップする
                if (parentRectTransformType == ParentRectTransformType.None) continue;
                var listItemUi = Instantiate(merchandiseListItemUiPrefab, GetParentListRootRectTransform(parentRectTransformType));
                listItemUi.InitializeUi(prize);
                // おまけに分類される場合はリストに代入する
                if (parentRectTransformType == ParentRectTransformType.Bonus)
                {
                    bonusMerchandiseListItemUiList.Add(listItemUi);
                }
            }

            //商品がある場合は商品の親オブジェクトを表示する
            productRoot.SetActive(productMerchandiseListItemUiList.Count > 0);
            //おまけがある場合はおまけの親オブジェクトを表示する
            bonusRoot.SetActive(bonusMerchandiseListItemUiList.Count > 0);
            
            // 自由記載枠の設定
            freeTextRoot.SetActive(!string.IsNullOrEmpty(detail.description));
            freeText.text = detail.description;
        }

        protected override void OnClosed()
        {
            modalData.OnUpdateUi?.Invoke();
            base.OnClosed();
        }

        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }

        public void OnClickTermsPaymentLaw()
        {
            PaymentLawModal.Open();
        }

        public void OnClickPurchaseButton()
        {
            ClickPurchaseButtonAsync().Forget();
        }

        private async UniTask ClickPurchaseButtonAsync()
        {
            // 購入時に年齢確認が必要な場合、年齢確認モーダルを表示する
            if (modalData.IsAgeConfirm)
            {
                if (await ShopManager.CheckAndShowBeforePurchaseConfirm(modalData.Bonus, modalData.PurchaseType) == false)
                {
                    return;
                }
            }
            modalData.OnClickPurchase?.Invoke();
            await CloseAsync();
            await ShopManager.OnClickPurchaseButton(modalData.Bonus, modalData.PurchaseType);
        }

        private ParentRectTransformType GetParentRectTransformType(string type, long id = 0)
        {
            switch (type)
            {
                case "point":
                    var mPoint = MasterManager.Instance.pointMaster.FindData(id);
                    if (mPoint == null) return ParentRectTransformType.None;
                    //0 => 特別な扱いはしない1 => おまけ（総付景品）
                    return mPoint.lawType == 0 ? ParentRectTransformType.Product : ParentRectTransformType.Bonus;
                case "chara":
                    return ParentRectTransformType.Bonus;
                case "charaPiece":
                    return ParentRectTransformType.Bonus;
                case "charaVariableTrainer":
                    return ParentRectTransformType.Bonus;
                case "icon":
                    return ParentRectTransformType.Product;
                case "title":
                    return ParentRectTransformType.Product;
                case "chatStamp":
                    return ParentRectTransformType.Product;
                default:
                    return ParentRectTransformType.None;
            }
        }

        private RectTransform GetParentListRootRectTransform(ParentRectTransformType type)
        {
            return type switch
            {
                ParentRectTransformType.None => null,
                ParentRectTransformType.Product => productRootRectTransform,
                ParentRectTransformType.Bonus => bonusRootRectTransform,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        // 購入方法の切り替え
        public void OnClickPurchaseChange()
        {
            List<BillingRewardBonusDetail> bonusList = new List<BillingRewardBonusDetail> { modalData.Bonus };
            modalData.PurchaseType = ShopManager.ChangePurchase(modalData.PurchaseType, bonusList);
            
            // 表示切替
            UpdateView();
        }
    }
}