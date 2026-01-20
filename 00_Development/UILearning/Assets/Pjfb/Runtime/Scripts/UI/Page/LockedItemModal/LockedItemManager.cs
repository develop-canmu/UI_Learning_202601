using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.LockedItem
{
    public static class LockedItemManager
    {
        #region バッジ用
        public static long unreceivedGiftCount;
        #endregion

        #region PublicFields
        private const string LockedItemLastShownFlag = "LockedItemLastShownFlag";
        public static bool LockedItemLastShownFlagPlayerPrefs
        {
            get => ObscuredPrefs.Get(key: LockedItemLastShownFlag, defaultValue: 0) == 1;
            set => ObscuredPrefs.Set(key: LockedItemLastShownFlag, value ? 1 : 0);
        }
        private const string LockedItemLastItemCount = "LockedItemLastItemCount";
        public static long LockedItemLastItemCountPlayerPrefs
        {
            get => ObscuredPrefs.Get(key: LockedItemLastItemCount, defaultValue: 0);
            set => ObscuredPrefs.Set(key: LockedItemLastItemCount, value);
        }
        #endregion
        
        
        #region StaticMethods
        public static void OpenLockedItemModalWindow(Action<long, long, string> onUpdatedLockParameters, Action<float> onReceivePresentComplete, float initialScrollValue, Action onWindowClosed = null)
        {
            CallGiftListApi(onSuccess: (response) =>
            {
                onUpdatedLockParameters.Invoke(response.unreceivedGiftLockedCount, response.unopenedGiftBoxCount, response.newestGiftLockedAt);
                
                LockedItemModalWindow.Open(new LockedItemModalWindow.Parameters(
                    lockedGiftListResponse: response,
                    onWindowClosed: onWindowClosed,
                    onClickReceiveItem: (presentModalWindow, id, lastScrollValue) => OnReceiveItem(presentModalWindow, id, lastScrollValue, onReceivePresentComplete, onUpdatedLockParameters),
                    onClickReceiveAll: (presentModalWindow, lastScrollValue) => OnReceiveAllItem(presentModalWindow, lastScrollValue, onReceivePresentComplete, onUpdatedLockParameters)
                    ));
            });
        }
        
        private static void OnReceiveItem(ModalWindow presentModalWindow, long id, float lastScrollValue, Action<float> onReceivePresentComplete, Action<long, long, string> onUpdatedLockParameters)
        {
            CallLockedGiftReceived(id, onSuccess: (response) =>
            {
                onUpdatedLockParameters?.Invoke(response.unreceivedGiftLockedCount, response.unopenedGiftBoxCount, response.newestGiftLockedAt);
                presentModalWindow.Close(onCompleted: () => RewardModal.TryOpen(new RewardModal.Parameters(response.prizeJsonList,
                    onWindowClosed: () => TryShowAutoSellConfirmModal(response.autoSell,
                        onFinish: () => onReceivePresentComplete?.Invoke(lastScrollValue)))));
            });
        }

        private static void OnReceiveAllItem(ModalWindow presentModalWindow, float lastScrollValue, Action<float> onReceivePresentComplete, Action<long, long, string> onUpdatedLockParameters)
        {
            CallAllLockedGiftReceived(onSuccess: (response) =>
            {
                onUpdatedLockParameters?.Invoke(response.unreceivedGiftLockedCount, response.unopenedGiftBoxCount, response.newestGiftLockedAt);
                presentModalWindow.Close(onCompleted: () => RewardModal.TryOpen(new RewardModal.Parameters(response.prizeJsonList, 
                    onWindowClosed: () => TryShowAutoSellConfirmModal(autoSell: response.autoSell,
                        onFinish: () => onReceivePresentComplete?.Invoke(lastScrollValue)))));
            });
        }

        private static async void TryShowAutoSellConfirmModal(NativeApiAutoSell autoSell, Action onFinish)
        {
            // 自動売却があるかどうかチェック
            if (autoSell?.prizeListGot != null && autoSell.prizeListSold != null &&
                (autoSell.prizeListGot.Length > 0 || autoSell.prizeListSold.Length > 0))
            {
                var autoSellModalData = new AutoSellConfirmModal.Data(autoSell);
                var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoSellConfirm, autoSellModalData);
                await modal.WaitCloseAsync();
            }
            onFinish?.Invoke();
        }

        private static async void CallLockedGiftReceived(long id, Action<GiftBoxReceiveLockedGiftAPIResponse> onSuccess)
        {
            var request = new GiftBoxReceiveLockedGiftAPIRequest();
            request.SetPostData(new GiftBoxReceiveLockedGiftAPIPost{uGiftBoxLockedId = id});
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            onSuccess.Invoke(response);
        }
        
        private static async void CallAllLockedGiftReceived(Action<GiftBoxReceiveLockedGiftAllAPIResponse> onSuccess)
        {
            var request = new GiftBoxReceiveLockedGiftAllAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            onSuccess.Invoke(response);
        }

        private static async void CallGiftListApi(Action<GiftBoxGetLockedGiftListAPIResponse> onSuccess)
        {
            var request = new GiftBoxGetLockedGiftListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            onSuccess.Invoke(response);
        }
        #endregion
    }
    
    public class ShownReleaseAnimationItemDataContainer
    {
        private const string ShowingOpenLockItems = "ShowingOpenLockItems";
        private static HashSet<long> _showingOpenLockItemsCache = null;
        private static HashSet<long> showingOpenLockItemIdsHashSetPlayerPrefs
        {
            get
            {
                if (_showingOpenLockItemsCache == null)
                {
                    var stringValue = ObscuredPrefs.Get(key: ShowingOpenLockItems, defaultValue: string.Empty);
                    var splitValue = stringValue.Split(',').ToList();
                    _showingOpenLockItemsCache = new HashSet<long>();
                    splitValue.ForEach(anIdString => {
                        if (int.TryParse(anIdString, out var anId)) _showingOpenLockItemsCache.Add(anId);
                    });
                }
                return _showingOpenLockItemsCache;
            }
            set {
                _showingOpenLockItemsCache = value;
                
                ObscuredPrefs.Set(key: ShowingOpenLockItems, _showingOpenLockItemsCache.ToList().ToCsv());
            }
        }

        public void Init(HashSet<long> openIds)
        {
            // 存在しないidを削除して掃除する処理
            showingOpenLockItemIdsHashSetPlayerPrefs = showingOpenLockItemIdsHashSetPlayerPrefs
                .Where(openIds.Contains)
                .ToHashSet();
        }
        
        public bool ShouldShowReleaseAnimation(long id)
        {
            return !showingOpenLockItemIdsHashSetPlayerPrefs.Contains(id);
        }

        public void OnFinishReleaseAnimation(long id)
        {
            var showingOpenLockItemIdsHashSet = showingOpenLockItemIdsHashSetPlayerPrefs;
            showingOpenLockItemIdsHashSet.Add(id);
            showingOpenLockItemIdsHashSetPlayerPrefs = showingOpenLockItemIdsHashSet;
        }
    }
}