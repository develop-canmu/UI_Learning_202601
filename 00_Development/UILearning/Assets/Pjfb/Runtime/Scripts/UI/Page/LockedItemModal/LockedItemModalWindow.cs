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

namespace Pjfb.LockedItem
{
    public class LockedItemModalWindow : ModalWindow
    {
        #region InnerClass
        public class Parameters
        {
            public GiftBoxGetLockedGiftListAPIResponse lockedGiftListResponse;
            public Action onWindowClosed;
            public Action<ModalWindow, long, float> onClickReceiveItem;
            public Action<ModalWindow, float> onClickReceiveAll;

            public Parameters(GiftBoxGetLockedGiftListAPIResponse lockedGiftListResponse, Action onWindowClosed, Action<ModalWindow, long, float> onClickReceiveItem, Action<ModalWindow, float> onClickReceiveAll)
            {
                this.lockedGiftListResponse = lockedGiftListResponse;
                this.onWindowClosed = onWindowClosed;
                this.onClickReceiveItem = onClickReceiveItem;
                this.onClickReceiveAll = onClickReceiveAll;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private UIButton receiveAllButton;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private GameObject notificationText;
        #endregion

        #region PrivateFields
        private Parameters _windowParams;
        private List<LockedItemPoolListItem.ItemParams> _showingListItemParams;
        private int _lastUpdateSecond = -1;
        private ShownReleaseAnimationItemDataContainer shownReleaseAnimationItemDataContainer = new();
        #endregion

        #region StaticMethods
        public static void Open(Parameters windowParams)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LockedItem, args: windowParams);
        }
        #endregion
        
        #region OverrideMethods
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (Parameters) args;
            ResetDisplay();
            return base.OnPreOpen(args, token);
        }
        
        protected override void OnClosed()
        {
            _windowParams?.onWindowClosed?.Invoke();
            ResetDisplay();
            base.OnClosed();
        }

        protected override UniTask OnPreClose(CancellationToken token)
        {
            poolListContainer.SlideOut().Forget();
            return base.OnPreClose(token);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            InitDisplay();
            return base.OnOpen(token);
        }

        /// <summary>
        /// このウインドウでのリストアイテムとウインドウのrealtimeで更新する処理はここでまとめた
        /// </summary>
        private void Update()
        {
            if (_showingListItemParams == null) return;
            
            var now = AppTime.Now;
            if (_lastUpdateSecond == now.Second) return;
            _lastUpdateSecond = now.Second;
            
            // リストアイテムを更新する処理
            var showingOpenLockItemIdsHashSet = shownReleaseAnimationItemDataContainer;
            _showingListItemParams.ForEach(anItem => {
                anItem.UpdateStatus(now);
                anItem.ActivePoolListItemInvoke<LockedItemPoolListItem>(anActiveListItem => anActiveListItem.UpdateDisplay(now, showingOpenLockItemIdsHashSet));
            });
            
            // ウインドウ内を表示する処理
            receiveAllButton.interactable = _showingListItemParams.Exists(anItem => anItem.realtimeStatus == LockedItemPoolListItem.Status.Unlocked);
        }
        #endregion

        #region PrivateMethods
        private void InitDisplay()
        {
            var giftList = _windowParams.lockedGiftListResponse.giftList.ToList();
            var now = AppTime.Now;
            
            _showingListItemParams = giftList
                .Select(aGift => new LockedItemPoolListItem.ItemParams(
                    id: aGift.uGiftBoxLockedId, 
                    prizeContainerDataList: aGift.prizeJson?.Select(PrizeJsonUtility.GetPrizeContainerData).ToList() ?? new List<PrizeJsonUtility.PrizeContainerData>(),
                    description: aGift.message,
                    startDateTime: aGift.openAt.TryConvertToDateTime(),
                    endDateTime: aGift.expireAt.TryConvertToDateTime(),
                    isReceivable: aGift.isReceivable,
                    onClickReceiveButton: OnClickReceiveItem))
                .Where(aGift => aGift.isReceivable && aGift.endDateTime.IsFuture(now))
                .OrderBy(aGift => aGift.startDateTime)
                .ToList();
            
            // 解放演出フラグ関連処理
            shownReleaseAnimationItemDataContainer.Init(_showingListItemParams.Select(aGift => aGift.id).ToHashSet());
            _showingListItemParams.ForEach(aGift => aGift.UpdateShownReleaseAnimationItemData(shownReleaseAnimationItemDataContainer));
            
            poolListContainer.gameObject.SetActive(true);
            poolListContainer.SetDataList(_showingListItemParams).Forget();
            notificationText.gameObject.SetActive(_showingListItemParams.Count <= 0);
            
            // メモ：PoolListItemの初期化が完了してからUpdateさせる
            UpdateNextFrame();
        }

        private void ResetDisplay()
        {
            _showingListItemParams = null;
            receiveAllButton.interactable = false;
            poolListContainer.gameObject.SetActive(false);
            poolListContainer.Clear();
        }

        private async void UpdateNextFrame()
        {
            await UniTask.DelayFrame(1);
            _lastUpdateSecond = -1;
        }
        #endregion
        
        #region EventListeners
        public void OnClickClose()
        {
            if (poolListContainer.isAnimating) return;
            Close(_windowParams.onWindowClosed);
        }

        private void OnClickReceiveItem(LockedItemPoolListItem.ItemParams itemParams)
        {
            if (poolListContainer.isAnimating) return;
            if (itemParams.prizeContainerDataList.Any(data => data.itemIconType == ItemIconType.SupportEquipment))
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }

            Logger.Log($"LockedItemModalWindow.OnClickReceiveItem id:{itemParams.id}");
            _windowParams?.onClickReceiveItem?.Invoke(this, itemParams.id, poolListContainer.scrollValue);
        }

        public void OnClickReceiveAll()
        {
            if (poolListContainer.isAnimating) return;
            var isContainingSupportEquipment = false;
            foreach (var itemParams in _showingListItemParams)
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
            
            Logger.Log($"LockedItemModalWindow.OnClickReceiveAll");
            _windowParams?.onClickReceiveAll?.Invoke(this, poolListContainer.scrollValue);
        }
        #endregion
    }
}
