using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Title;
using TMPro;
using UnityEngine;


namespace Pjfb.Menu
{
    public class InquiryConfirmModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public string inquiryTypeText = "";
            public string mailAddressText = "";
            public string bodyText = "";
            public Action onClosed;
        }

        [SerializeField] private TextMeshProUGUI inquiryTypeText;
        [SerializeField] private TextMeshProUGUI mailAddressText;
        [SerializeField] private TextMeshProUGUI bodyText;
        private WindowParams _windowParams;
        
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.InquiryConfirm, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args ?? new WindowParams();
            inquiryTypeText.text = _windowParams.inquiryTypeText;
            mailAddressText.text = _windowParams.mailAddressText;
            bodyText.text = _windowParams.bodyText;
            return base.OnPreOpen(args, token);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            // お知らせいTOPモーダルまでスタックから取り除く
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() != typeof(InquiryModal));
            Close(onCompleted: _windowParams.onClosed);
        }
        
        public void OnClickEdit()
        { 
            Close(onCompleted: _windowParams.onClosed);
        }
        
        public void OnClickPositiveButton()
        {
            SendAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        #endregion
        
        #region API
        private async UniTask SendAsync(CancellationToken token)
        {
            // 結果モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["title.inquiry"];
            // 閉じるボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.close"], 
                window => 
                {
                    // タイトルメニューモーダルまでスタックから取り除く
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(m => m.GetType() != typeof(TitleMenuModal) && m.GetType() != typeof(MenuModalWindow));
                    // 結果モーダルを閉じる
                    window.Close();
                }
            );
            
            try
            {
                // お問い合わせAPI
                if(string.IsNullOrEmpty(APIManager.Instance.setting.loginId) || string.IsNullOrEmpty(APIManager.Instance.setting.sessionId))
                {
                    // ログインしてない
                    InquirySendAPIRequest request = new InquirySendAPIRequest();
                    InquirySendAPIPost post = new InquirySendAPIPost();
                    post.appToken = LocalSaveManager.immutableData.appToken;
                    post.subject = inquiryTypeText.text;
                    post.body = bodyText.text;
                    post.mailFrom = mailAddressText.text;
                    post.deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo();
                    request.SetPostData(post);
                    await APIManager.Instance.Connect(request);
                }
                else
                {
                    // ログインしている
                    InquirySendDuringLoginAPIRequest request = new InquirySendDuringLoginAPIRequest();
                    InquirySendDuringLoginAPIPost post = new InquirySendDuringLoginAPIPost();
                    post.subject = inquiryTypeText.text;
                    post.body = bodyText.text;
                    post.mailFrom = mailAddressText.text;
                    post.deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo();
                    request.SetPostData(post);
                    await APIManager.Instance.Connect(request);
                }
                
                // メッセージ
                data.Message = StringValueAssetLoader.Instance["title.inquiry_success"];
                // 結果モーダル開く
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            }
            catch (APIException e)
            {
                CruFramework.Logger.LogError(e.Message);
            }
        }
        #endregion
    }
}