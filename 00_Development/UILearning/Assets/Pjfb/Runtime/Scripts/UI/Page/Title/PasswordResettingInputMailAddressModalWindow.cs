using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb
{
    public class PasswordResettingInputMailAddressModalWindow : ModalWindow
    {
        [SerializeField] private TMP_InputField mailAddressInputField;
        [SerializeField] private UIButton startResetPasswordButton;

        protected override UniTask OnOpen(CancellationToken token)
        {
            OnInputFieldValueChangeMailAddress(string.Empty);
            return base.OnOpen(token);
        }

        public async void OnClickStartRestPasswordButton()
        {
            // パスワード変更開始
            UserEmailPasswordResetStartAPIRequest request = new UserEmailPasswordResetStartAPIRequest();
            UserEmailPasswordResetStartAPIPost post = new UserEmailPasswordResetStartAPIPost() { mailAddress = mailAddressInputField.text };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            // メールアドレスを渡す
            PasswordResettingModalWindow.Data passwordResettingData = new PasswordResettingModalWindow.Data();
            passwordResettingData.UserMailAddress = mailAddressInputField.text;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PasswordResetting,passwordResettingData);
        }

        public void OnInputFieldValueChangeMailAddress(string input)
        {
            mailAddressInputField.SetTextWithoutNotify(input);
            startResetPasswordButton.interactable = !string.IsNullOrEmpty(mailAddressInputField.text);
        }
        
    }
}