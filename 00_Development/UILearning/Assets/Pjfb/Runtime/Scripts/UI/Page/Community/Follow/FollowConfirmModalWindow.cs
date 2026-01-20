using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class FollowConfirmModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public bool SendYellEnable = true;
            public long UMasterId;
            public string UserName;
            public string BadgeCountText;
            public Action onClosed;
            public Action OnClickUnfollow;
        }

        [SerializeField] private UIButton confirmButton;
        [SerializeField] private GameObject yellCountBadge;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageUpText;
        [SerializeField] private TextMeshProUGUI messageDownText;
        [SerializeField] private TextMeshProUGUI yellCountBadgeText;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private Image confirmButtonImage;
        [SerializeField] private Sprite confirmSprite;
        [SerializeField] private Sprite unblockSprite;

        private WindowParams _windowParams;

        #endregion
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.FollowConfirm, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _windowParams.onClosed?.Invoke();
        }

        #region PrivateMethods
        private void Init()
        {
            if (string.IsNullOrEmpty(_windowParams.BadgeCountText))
            {
                titleText.text = StringValueAssetLoader.Instance["community.unfollow"];
                messageUpText.text = StringValueAssetLoader.Instance["community.follow.unfollow_confirm"];
                confirmButtonText.text = StringValueAssetLoader.Instance["community.unfollow_confirm_button"];
                messageDownText.gameObject.SetActive(false);
                yellCountBadge.SetActive(false);
                confirmButtonImage.sprite = unblockSprite;
            }
            else
            {
                titleText.text = StringValueAssetLoader.Instance["community.yell.send_title"];
                messageUpText.text = string.Format(StringValueAssetLoader.Instance["community.yell.send_confirm"],_windowParams.UserName);
                confirmButtonText.text = StringValueAssetLoader.Instance["community.yell.send"];
                yellCountBadgeText.text = _windowParams.BadgeCountText;
                messageDownText.gameObject.SetActive(true);
                yellCountBadge.SetActive(true);
                confirmButtonImage.sprite = confirmSprite;
            }

            confirmButton.interactable = _windowParams.SendYellEnable;
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }

        public void OnClickConfirm()
        {
            if (!string.IsNullOrEmpty(_windowParams.UserName) && _windowParams.UMasterId != 0)
            {
                SendYellAPI(_windowParams.UMasterId).Forget();
            }else if (_windowParams.OnClickUnfollow != null)
            {
                _windowParams.OnClickUnfollow.Invoke();
                Close();
            }
        }

        #endregion

        #region API
        private async UniTask SendYellAPI(long targetUMasterId = 0)
        {
            CommunitySendYellAPIRequest request = new CommunitySendYellAPIRequest();
            CommunitySendYellAPIPost post = new CommunitySendYellAPIPost{targetUMasterId = targetUMasterId,category = 0};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CommunitySendYellAPIResponse response = request.GetResponseData();
            long getYellPoint = response.yellResult.yellPointList.ToList().Select(x => x.value).Sum();

            YellSendModalWindow.Open(new YellSendModalWindow.WindowParams
            {
                UMasterId = targetUMasterId,
                UserName = _windowParams.UserName,
                YellPoint = getYellPoint
            });
        }
        #endregion
    }
}