using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Community
{
    public class YellSendModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public long UMasterId;
            public string UserName;
            public long YellPoint;
            public Action onClosed;
        }

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI userGotPointText;
        [SerializeField] private TextMeshProUGUI gotPointCountText;
        [SerializeField] private TextMeshProUGUI inputLimitCountText;
        [SerializeField] private TextMeshProUGUI showText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private UIButton sendButton;

        private WindowParams _windowParams;
        
        #endregion
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.YellSend, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            inputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, inputField.characterLimit,inputField.fontAsset);
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            messageText.text =  string.Format(StringValueAssetLoader.Instance["community.yell.sent_confirm"],_windowParams.UserName);
            userGotPointText.text = string.Format(StringValueAssetLoader.Instance["community.yell.got_point"],_windowParams.UserName);
            gotPointCountText.text = $"+{_windowParams.YellPoint.ToString("N0")}";
            OnInputFieldValueChanged("");
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() == typeof(FollowConfirmModalWindow));
            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickSendMessage()
        {
            if (!string.IsNullOrEmpty(inputField.text)) SendChat(inputField.text,_windowParams.UMasterId).Forget();
        }
        
        public void OnInputFieldValueChanged(string input)
        {
            input = StringUtility.GetLimitNumCharacter(input,inputField.characterLimit);
            inputField.SetTextWithoutNotify(input);
            sendButton.interactable = !string.IsNullOrEmpty(input);
            inputLimitCountText.text = $"{input?.Length ?? 0}/{inputField.characterLimit}";
            showText.text = input;
        }
        
        public void OnClickInputField()
        {
            inputField.ActivateInputField();
        }

        #endregion

        #region API

        private async UniTask SendChat(string body,long targetUMasterId)
        {
            inputField.text = "";
            CommunitySendChatAPIRequest request = new CommunitySendChatAPIRequest();
            CommunitySendChatAPIPost post = new CommunitySendChatAPIPost
            {
                type = (int) MessageType.Message,
                body = body,
                targetUMasterId = targetUMasterId,
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close(onCompleted: () =>
            {
                var info = new CommunityPage.CommunityPageInfo{ Status = CommunityStatus.PersonalChat, TargetUMasterId = targetUMasterId};
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, info);
            });
        }

        #endregion
        
    }
}