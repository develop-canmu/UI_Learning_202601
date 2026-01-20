using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.PresentBox
{
    public class PresentBoxModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public GiftBoxGetGiftListAPIResponse presentResponse;
            public GiftBoxGetGiftHistoryListAPIResponse nullableHistoryResponse;
            public Action<ModalWindow, long, float> onReceivePresent;
            public Action<ModalWindow, float> onReceiveAllPresent;
            public Action onClosed;

            public int selectedIndex { get; private set; } = 0;
            public void SetSelectedIndex(int index) => selectedIndex = index;
            
            public float initialScrollValue { get; set; } = 1f;
            public void SetInitialScrollValue(float scrollValue) => initialScrollValue = scrollValue;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private UIButton receiveAllButton;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private TMPro.TMP_Text notificationText;
        [SerializeField] private ToggleContainer toggleContainer;
        #endregion

        #region PrivateFields
        private WindowParams _windowParams;
        private List<PresentBoxPoolListItem.ItemParams> presentPoolItemParamList;
        #endregion

        #region StaticMethods
        public static void Open(WindowParams windowParams)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PresentBox, args: windowParams);
        }
        #endregion
        
        #region OverrideMethods
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            ResetDisplay();
            return base.OnPreOpen(args, token);
        }
        
        protected override void OnClosed()
        {
            _windowParams?.onClosed?.Invoke();
            ResetDisplay();
            base.OnClosed();
        }

        protected override UniTask OnPreClose(CancellationToken token)
        {
            _windowParams.SetInitialScrollValue(poolListContainer.scrollValue);
            poolListContainer.SlideOut().Forget();
            return base.OnPreClose(token);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            InitDisplay();
            return base.OnOpen(token);
        }
        #endregion

        #region PrivateMethods
        private async void InitDisplay()
        {
            var initialIndexDisplay = _windowParams.selectedIndex;
            toggleContainer.Init(initialIndexDisplay, OnClickTab);
            await ShowTab(initialIndexDisplay, scrollValue: _windowParams.initialScrollValue);
        }

        private async UniTask TryShowHistory(float scrollValue)
        {
            if (_windowParams.nullableHistoryResponse == null) {
                await PresentBoxManager.CallGiftListHistory(onSuccess: response => _windowParams.nullableHistoryResponse = response);
            }
            
            var historyResponse = _windowParams.nullableHistoryResponse;
            presentPoolItemParamList = historyResponse.giftList.Select(aGift =>
            {
                var prizeContainerDataList = aGift.prizeJson?.Select(PrizeJsonUtility.GetPrizeContainerData).ToList() ?? new List<PrizeJsonUtility.PrizeContainerData>();
                var dateText = string.Empty;
                var dateTime = aGift.receivedAt.TryConvertToDateTime();
                if (dateTime != DateTime.MinValue)
                {
                    dateText = dateTime.GetNewsDateTimeString();
                }
                
                return new PresentBoxPoolListItem.ItemParams(
                    id: 0,
                    prizeContainerDataList: prizeContainerDataList,
                    description: aGift.message,
                    showReceiveButton: false,
                    dateText: dateText,
                    onClickReceiveButton: OnClickReceivePresent);
            }).ToList();

            receiveAllButton.interactable = false;
            await SetDataList(presentPoolItemParamList, StringValueAssetLoader.Instance["present.history.empty"], scrollValue);
        }

        private async UniTask SetDataList<T>(IList<T> poolItemParamList, string emptyText, float scrollValue) where T:PoolListItemBase.ItemParamsBase
        {
            var isEmpty = poolItemParamList.IsNullOrEmpty();
            notificationText.text = isEmpty ? emptyText : string.Empty;
            notificationText.gameObject.SetActive(isEmpty);
            poolListContainer.gameObject.SetActive(!isEmpty);
            if (!isEmpty) await poolListContainer.SetDataList(poolItemParamList, scrollValue: scrollValue);
        }

        private async UniTask ShowPresentList(float scrollValue)
        {
            var presentResponse = _windowParams.presentResponse;
            presentPoolItemParamList = presentResponse.giftList.Select(aGift =>
            {
                var prizeContainerDataList = aGift.prizeJson?.Select(PrizeJsonUtility.GetPrizeContainerData).ToList() ?? new List<PrizeJsonUtility.PrizeContainerData>();
                var dateText = string.Empty;
                var dateTime = aGift.expireAt.TryConvertToDateTime();
                if (dateTime != DateTime.MinValue)
                {
                    dateText = dateTime.GetRemainingString(pastDate: AppTime.Now, remainingDayLimit: 364, textFormat: "あと{0}");
                }

                return new PresentBoxPoolListItem.ItemParams(
                    id: aGift.uGiftBoxId,
                    prizeContainerDataList: prizeContainerDataList,
                    description: aGift.message,
                    showReceiveButton: aGift.isReceivable,
                    dateText: dateText,
                    onClickReceiveButton: OnClickReceivePresent);
            }).ToList();
            
            receiveAllButton.interactable = presentPoolItemParamList.Any();
            await SetDataList(presentPoolItemParamList, StringValueAssetLoader.Instance["present.list.empty"], scrollValue);
        }

        private void ResetDisplay()
        {
            notificationText.gameObject.SetActive(false);
            receiveAllButton.interactable = false;
            poolListContainer.gameObject.SetActive(false);
            poolListContainer.Clear();
        }
        #endregion
        
        #region EventListeners
        public void OnClickClose()
        {
            if (poolListContainer.isAnimating) return;
            Close();
        }

        private void OnClickReceivePresent(PresentBoxPoolListItem.ItemParams itemParams)
        {
            if (poolListContainer.isAnimating) return;
            if (itemParams.prizeContainerDataList.Any(data => data.itemIconType == ItemIconType.SupportEquipment))
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }

            Logger.Log($"PresentBoxModalWindow.OnClickReceivePresent presentId:{itemParams.id}");
            _windowParams?.onReceivePresent?.Invoke(this, itemParams.id, poolListContainer.scrollValue);
        }

        public void OnClickReceiveAllPresent()
        {
            if (poolListContainer.isAnimating) return;
            var isContainingSupportEquipment = false;
            foreach (var itemParams in presentPoolItemParamList)
            {
                if (itemParams.prizeContainerDataList.Any(data => data.itemIconType == ItemIconType.SupportEquipment))
                {
                    isContainingSupportEquipment = true;
                    break;
                }
            }
            if (isContainingSupportEquipment)
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }
            
            Logger.Log($"PresentBoxModalWindow.OnClickReceiveAllPresent");
            _windowParams?.onReceiveAllPresent?.Invoke(this, poolListContainer.scrollValue);
        }

        private async UniTask OnClickTab(int index)
        {
            await poolListContainer.SlideOut();
            await ShowTab(index);
            _windowParams.SetSelectedIndex(index);
        }

        private async UniTask ShowTab(int index, float scrollValue = 1f)
        {
            switch (index) {
                case 0: await ShowPresentList(scrollValue); break;
                case 1: await TryShowHistory(scrollValue); break;
            }
        }
        #endregion
    }
}
