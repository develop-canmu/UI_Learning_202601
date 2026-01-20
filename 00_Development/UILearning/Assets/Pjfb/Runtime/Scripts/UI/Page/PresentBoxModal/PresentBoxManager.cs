using System;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.PresentBox
{
    public static class PresentBoxManager
    {
        #region バッジ用
        public static long unreceivedGiftCount;
        #endregion
        
        #region StaticMethods
        public static void OpenPresentBoxModalWindow(Action<long, long, long, string, PrizeJsonWrap[], float> onReceivePresentComplete, float initialScrollValue, Action onWindowClosed = null)
        {
            CallGiftListApi(onSuccess: (response) =>
            {
                PresentBoxModalWindow.Open(new PresentBoxModalWindow.WindowParams
                {
                    presentResponse = response,
                    nullableHistoryResponse = null, // メモ：APIを軽くするために、履歴タブをタップした際に履歴APIを叩く
                    onClosed = onWindowClosed,
                    initialScrollValue = initialScrollValue,
                    onReceivePresent = (presentModalWindow, id, lastScrollValue) => OnReceivePresent(presentModalWindow, id, lastScrollValue, onReceivePresentComplete),
                    onReceiveAllPresent = (presentModalWindow, lastScrollValue) => OnReceiveAllPresent(presentModalWindow, lastScrollValue, onReceivePresentComplete)
                });
            });
        }
        
        private static void OnReceivePresent(ModalWindow presentModalWindow, long id, float lastScrollValue, Action<long, long, long, string, PrizeJsonWrap[], float> onReceivePresentComplete)
        {
            CallGiftReceived(id, onSuccess: (response) =>
            {
                presentModalWindow.Close(onCompleted: () => RewardModal.TryOpen(new RewardModal.Parameters(response.prizeJsonList,
                    onWindowClosed: () => TryShowAutoSellConfirmModal(response.autoSell, 
                        onFinish: () => onReceivePresentComplete?.Invoke(
                            response.unreceivedGiftCount,
                            response.unreceivedGiftLockedCount,
                            response.unopenedGiftBoxCount,
                            response.newestGiftLockedAt,
                            response.prizeJsonList,
                            lastScrollValue)))));
            });
        }

        private static void OnReceiveAllPresent(ModalWindow presentModalWindow, float lastScrollValue, Action<long, long, long, string, PrizeJsonWrap[], float> onReceivePresentComplete)
        {
            CallAllGiftReceived(onSuccess: (response) =>
            {
                presentModalWindow.Close(onCompleted: () => RewardModal.TryOpen(new RewardModal.Parameters(response.prizeJsonList, 
                    onWindowClosed: () => TryShowAutoSellConfirmModal(autoSell: response.autoSell,
                        onFinish: () => onReceivePresentComplete?.Invoke(
                            response.unreceivedGiftCount,
                            response.unreceivedGiftLockedCount,
                            response.unopenedGiftBoxCount,
                            response.newestGiftLockedAt,
                            response.prizeJsonList,
                            lastScrollValue)))));
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

        private static async void CallGiftReceived(long id, Action<GiftBoxReceiveAPIResponse> onSuccess)
        {
            var request = new GiftBoxReceiveAPIRequest();
            request.SetPostData(new GiftBoxReceiveAPIPost{uGiftBoxId = id});
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            onSuccess.Invoke(response);
        }
        
        private static async void CallAllGiftReceived(Action<GiftBoxReceiveAllAPIResponse> onSuccess)
        {
            var request = new GiftBoxReceiveAllAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            onSuccess.Invoke(response);
        }

        private static async void CallGiftListApi(Action<GiftBoxGetGiftListAPIResponse> onSuccess)
        {
            var request = new GiftBoxGetGiftListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            onSuccess.Invoke(response);
        }
        
        public static async UniTask CallGiftListHistory(Action<GiftBoxGetGiftHistoryListAPIResponse> onSuccess)
        {
            var request = new GiftBoxGetGiftHistoryListAPIRequest();
            request.SetPostData(new GiftBoxGetGiftHistoryListAPIPost{page = 1});
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            onSuccess.Invoke(response);
        }
        #endregion
    }
}