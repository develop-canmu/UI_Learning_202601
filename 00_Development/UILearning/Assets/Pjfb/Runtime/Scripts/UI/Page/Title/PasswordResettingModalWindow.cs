using System;
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
    public class PasswordResettingModalWindow : ModalWindow
    {
        public class Data
        {
            private string userMailAddress = String.Empty;
            public string UserMailAddress
            {
                get { return userMailAddress; }
                set { userMailAddress = value; }
            }
        }

        private const int PasswordLowerLimit = 5;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField passwordAgainInputField;
        [SerializeField] private TMP_InputField authCodeInputField;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private UIButton authCodeReSendButton;
        [SerializeField] private GameObject passwordPlaceholderAnnotation;
        [SerializeField] private GameObject passwordRightAnnotation;
        [SerializeField] private GameObject passwordLimitAnnotation;
        private Data passwordResettingData;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            passwordResettingData = (Data)args;
            
            InputFieldValueChangedPassword(String.Empty);
            InputFieldValueChangedPasswordAgain(String.Empty);
            InputFieldValueChangedAuthCode(String.Empty);
            return base.OnPreOpen(args, token);
        }

        public async void OnClickApplyButton()
        {
            // パスワード変更
            UserEmailPasswordResetFinishAPIRequest request = new UserEmailPasswordResetFinishAPIRequest();
            UserEmailPasswordResetFinishAPIPost post = new UserEmailPasswordResetFinishAPIPost()
            {
                mailAddress = this.passwordResettingData.UserMailAddress,
                password = passwordInputField.text,
                authCode = authCodeInputField.text,
                deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo(),
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            // パスワードの再設定完了後モーダル表示
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["transfer.password.reset.complete_title"],
                StringValueAssetLoader.Instance["transfer.password.reset.complete_message"],
                null, 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (v) =>
                {
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window =>
                        window.GetType() != typeof(UserTransferModal));
                    v.Close();
                }));
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        public async void OnClickAuthCodeReSendButton()
        {
            UserEmailPasswordResetStartAPIRequest request = new UserEmailPasswordResetStartAPIRequest();
            UserEmailPasswordResetStartAPIPost post = new UserEmailPasswordResetStartAPIPost() { mailAddress = this.passwordResettingData.UserMailAddress };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // 認証コードが再送信されたらモーダルを表示
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["transfer.authcode.resend_title"],
                StringValueAssetLoader.Instance["transfer.authcode.resend_message"],
                StringValueAssetLoader.Instance["transfer.authcode.resend_caution"], 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (v) => { v.Close(); }));
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            authCodeReSendButton.SetClickIntervalTimer(authCodeReSendButton.ClickTriggerInterval);
        }

        public void InputFieldValueChangedPassword(string input)
        {
            passwordInputField.SetTextWithoutNotify(input);
            CheckInput();
        }

        public void InputFieldValueChangedPasswordAgain(string input)
        {
            passwordAgainInputField.SetTextWithoutNotify(input);
            CheckInput();
        }

        public void InputFieldValueChangedAuthCode(string input)
        {
            authCodeInputField.SetTextWithoutNotify(input);
            CheckInput();
        }

        private void CheckInput()
        {
            bool hasInputPassword = !string.IsNullOrEmpty(passwordInputField.text);
            bool hasInputPasswordAgain = !string.IsNullOrEmpty(passwordAgainInputField.text);
            bool hasInputAuthCode = !string.IsNullOrEmpty(authCodeInputField.text);
            // ボタンを押してもいいか判定
            if (!hasInputPassword && !hasInputPasswordAgain)
            {
                passwordPlaceholderAnnotation.SetActive(false);
                passwordRightAnnotation.SetActive(false);
                passwordLimitAnnotation.SetActive(false);
                applyButton.interactable = false;
            }
            else if (!hasInputPassword || !hasInputPasswordAgain)
            {
                passwordPlaceholderAnnotation.SetActive(true);
                passwordRightAnnotation.SetActive(false);
                passwordLimitAnnotation.SetActive(false);
                applyButton.interactable = false;
            }
            else if(passwordInputField.text != passwordAgainInputField.text)
            {
                passwordPlaceholderAnnotation.SetActive(false);
                passwordRightAnnotation.SetActive(true);
                passwordLimitAnnotation.SetActive(false);
                applyButton.interactable = false;
            }
            else if (passwordInputField.text.Length < PasswordLowerLimit)
            {
                passwordPlaceholderAnnotation.SetActive(false);
                passwordRightAnnotation.SetActive(false);
                passwordLimitAnnotation.SetActive(true);
                applyButton.interactable = false;
            }
            else if (!hasInputAuthCode)
            {
                passwordPlaceholderAnnotation.SetActive(false);
                passwordRightAnnotation.SetActive(false);
                passwordLimitAnnotation.SetActive(false);
                applyButton.interactable = false;
            }
            else
            {
                passwordPlaceholderAnnotation.SetActive(false);
                passwordRightAnnotation.SetActive(false);
                passwordLimitAnnotation.SetActive(false);
                applyButton.interactable = true;
            }
        }
        
        
    }
}