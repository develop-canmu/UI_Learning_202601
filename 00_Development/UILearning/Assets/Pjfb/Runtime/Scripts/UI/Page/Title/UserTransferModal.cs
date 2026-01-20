using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Title;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class UserTransferModal : ModalWindow
    {
        [SerializeField]
        private TMP_InputField loginIdInput = null;
        
        [SerializeField]
        private TMP_InputField passwordInput = null;

        [SerializeField] 
        private UIButton transferButton;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            loginIdInput.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, loginIdInput.characterLimit,loginIdInput.fontAsset);
            passwordInput.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, passwordInput.characterLimit,passwordInput.fontAsset);
            return base.OnPreOpen(args, token);
        }
        
        protected override UniTask OnOpen(CancellationToken token)
        {
            loginIdInput.text = string.Empty;
            passwordInput.text = string.Empty;
            CheckText();
            return base.OnOpen(token);
        }
        
        private async UniTask UserTransferAsync(CancellationToken token)
        {
            // API
            UserTransferAPIRequest request = new UserTransferAPIRequest();
            UserTransferAPIPost post = new UserTransferAPIPost();
            post.loginId = loginIdInput.text;
            post.password = passwordInput.text;
            post.deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            // 結果モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["title.user_transfer"];
            // メッセージ
            data.Message = StringValueAssetLoader.Instance["title.user_transfer_success"];
            // 注意
            data.Caution = StringValueAssetLoader.Instance["title.user_transfer_success_caution"];
            // 閉じるボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.close"],
                window =>
                {
                    // モーダルクリア
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(_ => true);
                    // 結果モーダルを閉じる、再起動
                    window.Close(()=>AppManager.Instance.BackToTitle());
                }
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickPositiveButton()
        {
            UserTransferAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>uGUI</summary>
        public void OnClickNegativeButton()
        {
            Close();
        }

        public void OnClickResetPasswordButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PasswordResettingInputMailAddress,null);
        }
        
        /// <summary>uGUI</summary>
        public void OnLoginIdEditEnd(string input)
        {
            string loginId = StringUtility.GetLimitNumCharacter(input, loginIdInput.characterLimit);
            loginIdInput.text = loginId;
        }
        
        /// <summary>uGUI</summary>
        public void OnPasswordEditEnd(string input)
        {
            string password = StringUtility.GetLimitNumCharacter(input, passwordInput.characterLimit);
            passwordInput.text = password;
        }

        public void CheckText()
        {
            if (!string.IsNullOrEmpty(loginIdInput.text) && !string.IsNullOrEmpty(passwordInput.text))
            {
                transferButton.interactable = true;
            }
            else
            {
                transferButton.interactable = false;
            }
        }
    }
}
