using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;
using Pjfb.Common;

namespace Pjfb.Shop
{
    public class ShopExchangeConfirmModal : ModalWindow
    {
        private const int MAX_EXCHANGE_COUNT = 100;
        private const int MIN_EXCHANGE_COUNT = 1;
        public class ModalParams
        {
            public CommonStoreMasterObject storeItem;
            public long exchangedCount;
            public Action<List<StoreBuyingInfo>> OnExchangedItem;
        }

        [SerializeField] private ItemIconContainer costIcon;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private PossessionItemUi requiredItemUi;
        [SerializeField] private PossessionItemUi possessionItemUi;
        [SerializeField] private UiSlider costSlider;
        [SerializeField] private UIButton exchangeButton;
        [SerializeField] private TMP_Text titleText;
        
        private CommonStoreMasterObject commonStore;

        private long exchangedCount;
        private bool iconLoaded;
        private bool isSlider;
        private PrizeJsonWrap firstViewPrize;
        private Action<List<StoreBuyingInfo>> OnExchangedItem;

        public static void Open(CommonStoreMasterObject commonStore, long exchangedCount, Action<List<StoreBuyingInfo>> onExchangedItem)
        {
            var param = new ModalParams();
            var point = MasterManager.Instance.pointMaster.FindData(commonStore.costMPointId);
            CruFramework.Logger.Log($"ユーザーID{UserDataManager.Instance.user.uMasterId}");
            CruFramework.Logger.Log($"コスト:{point?.name}/{commonStore.costMPointId}/{commonStore.costValue}");
            param.storeItem = commonStore;
            param.exchangedCount = exchangedCount;
            param.OnExchangedItem = onExchangedItem;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopExchangeConfirm, param);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var modalParams = (ModalParams)args;

            firstViewPrize = modalParams.storeItem.prizeJson.FirstOrDefault();
            commonStore = modalParams.storeItem;
            exchangedCount = modalParams.exchangedCount;
            OnExchangedItem = modalParams.OnExchangedItem;
            
            InitializeCostUi();
            InitializeSliderUi();
            
            return base.OnPreOpen(args, token);
        }
        
        private void InitializeCostUi()
        {
            titleText.text = commonStore.name;
            prizeJsonView.SetView(firstViewPrize);
            costIcon.SetIcon(ItemIconType.Item,commonStore.costMPointId);
            costIcon.SetCount(commonStore.costValue);
            requiredItemUi.SetRequiredCount(commonStore.costMPointId,commonStore.costValue);
            possessionItemUi.SetAfterCountByAmount(commonStore.costMPointId, commonStore.costValue);
        }
        
        private void InitializeSliderUi()
        {
            var costValue = commonStore.costValue;
            var possessionCount = UserDataManager.Instance.point.Find(commonStore.costMPointId)?.value ?? 0;
            exchangeButton.interactable = possessionCount >= costValue;
            
            isSlider = commonStore.buyLimit != MIN_EXCHANGE_COUNT;
            long exchangeableCount = 0;
            
            
            switch (commonStore.buyLimit)
            {
                case 0:
                    // 交換制限なしの交換上限はドラスマ準拠で100
                    if(costValue > 0)
                    {
                        exchangeableCount = Mathf.Clamp(Mathf.FloorToInt((float)possessionCount / costValue), MIN_EXCHANGE_COUNT, MAX_EXCHANGE_COUNT);
                    }
                    else
                    {
                        exchangeableCount = MAX_EXCHANGE_COUNT;
                    }
                    break;
                default:
                    // 制限ありは購入個数分減算する
                    if(costValue > 0)
                    {
                        exchangeableCount = Math.Max(0, Math.Min(Mathf.FloorToInt((float)possessionCount / costValue), commonStore.buyLimit - exchangedCount));
                    }
                    else
                    {
                        exchangeableCount = commonStore.buyLimit - exchangedCount;
                    }
                    break;
            }

            var sliderDefaultValueType = MasterManager.Instance.commonStoreCategoryMaster.FindData(commonStore.mCommonStoreCategoryId)?.sliderDefaultValueType ?? 0;
            var isMax = sliderDefaultValueType == 1;
            if (isMax && isSlider)
            {
                costSlider.SetSliderRange(1, exchangeableCount);
                if ((int)costSlider.GetSliderValue() == exchangeableCount)
                {
                    OnValueChangedSlider();   
                }
                else
                {
                    costSlider.SetSliderValue(exchangeableCount);
                }
            }
            else
            {
                costSlider.SetSliderRange(1, exchangeableCount);
                if ((int)costSlider.GetSliderValue() == MIN_EXCHANGE_COUNT)
                {
                    OnValueChangedSlider();
                }
                else
                {
                    costSlider.SetSliderValue(MIN_EXCHANGE_COUNT);   
                }
            }
            costSlider.gameObject.SetActive(isSlider);
            costSlider.SetButtonInteractable();
        }
        
        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        
        public void OnClickExchangeButton()
        {
            RequestShopBuyStoreApi().Forget();
        }
        
        public void OnValueChangedSlider()
        {
            var exchangeCount = (int)costSlider.GetSliderValue();
            var requireCount = exchangeCount * commonStore.costValue;
            requiredItemUi.UpdateRequiredCount(requireCount);
            possessionItemUi.UpdateAfterAmount(-requireCount);
            var value = firstViewPrize?.args?.value ?? 1;
            prizeJsonView.SetCount(value * exchangeCount);
            costIcon.SetCount(requireCount);
            
        }
        #endregion
        
        #region Api
        private async UniTask RequestShopBuyStoreApi()
        {
            int sliderValue = (int)costSlider.GetSliderValue();
            //Token取得
            var token = await Pjfb.Networking.App.APIUtility.ConnectOneTimeTokeRequest();

            var request = new ShopBuyStoreAPIRequest();
            request.oneTimeToken = token.oneTimeToken;
            var post = new ShopBuyStoreAPIPost
            { 
                idList = new long[] {commonStore.id},
                countList = new long[] {sliderValue}
            };
            request.SetPostData(post);
            
            try
            {
                await APIManager.Instance.Connect(request);
                OnExchangedItem?.Invoke(await ShopPage.GetShopGetStoreBuyingInfoAPI());

                var exchangedItem = new PrizeJsonWrap(firstViewPrize);
                exchangedItem.args.value = (firstViewPrize?.args?.value ?? 1) * sliderValue;

                ShopExchangeSuccessConfirmModal.Open(new ShopExchangeSuccessConfirmModal.Parameters(new List<PrizeJsonWrap>(){exchangedItem},
                    () =>
                    {
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((window) => window is not ShopExchangeModal);
                    }));
            }
            catch
            {
                CruFramework.Logger.LogError("ShopBuyStoreAPIRequest error");
            }
        }
        #endregion

    }
   
}