using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Community;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class UserTransferSettingModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public Action onClosed;
        }

        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField passwordAgainInputField;
        [SerializeField] private TMP_InputField mailAddressInputField;
        [SerializeField] private TMP_InputField authCodeInputField;
        [SerializeField] private TextMeshProUGUI loginIdText;
        [SerializeField] private TextMeshProUGUI inputWarningText;
        [SerializeField] private TextMeshProUGUI displayMailAddressTMP;
        [SerializeField] private TextMeshProUGUI displayMailAddress;
        [SerializeField] private GameObject loginIdRoot;
        [SerializeField] private GameObject inputPassRoot;
        [SerializeField] private GameObject inputMailAddressRoot;
        [SerializeField] private GameObject inputAuthMailRoot;
        [SerializeField] private GameObject linkMailAddress;
        [SerializeField] private GameObject linkedMailAddress;
        [SerializeField] private GameObject registeredMailAddress;
        [SerializeField] private GameObject unregisteredMailAddress;
        [SerializeField] private GameObject registerButton;
        [SerializeField] private GameObject changeButton;
        [SerializeField] private GameObject closeButton;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private UIButton applyMailAddressButton;
        [SerializeField] private UIButton authCodeReSendButton;
        [SerializeField] private UIButton authCodeTransferButton;
        [SerializeField] private UIButton backButton;
        [SerializeField] private UINotification notificationUI;
        
        private WindowParams _windowParams;
        private UserGetTransferSettingAPIResponse currentResponse;

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserTransferSetting, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await Init();
            await base.OnPreOpen(args, token);
        }
        private async UniTask Init()
        {
            currentResponse = await UserGetTransferSettingAPI();
            bool hasPassword = currentResponse.hasPassword == "true";
            loginIdText.text = currentResponse.loginId;
            bool hasMailAddress = !string.IsNullOrEmpty(currentResponse.mailAddress);
            if (hasMailAddress)
            {
                displayMailAddress.text = currentResponse.mailAddress;
            }
            inputWarningText.text = "";
            OnInputFieldValueChanged("");
            OnAgainInputFieldValueChanged("");
            registerButton.SetActive(!hasMailAddress);
            changeButton.SetActive(hasMailAddress);
            linkMailAddress.SetActive(!hasMailAddress);
            linkedMailAddress.SetActive(hasMailAddress);
            registeredMailAddress.SetActive(hasMailAddress);
            unregisteredMailAddress.SetActive(!hasMailAddress);
            displayMailAddress.gameObject.SetActive(hasMailAddress);
            loginIdRoot.SetActive(hasPassword);
            inputPassRoot.SetActive(!hasPassword);
            closeButton.SetActive(true);
            inputMailAddressRoot.SetActive(false);
            inputAuthMailRoot.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        public void OnInputFieldValueChanged(string input)
        {
            passwordInputField.SetTextWithoutNotify(input);
            CheckInputPassword();
        }

        public void OnAgainInputFieldValueChanged(string input)
        {
            passwordAgainInputField.SetTextWithoutNotify(input);
            CheckInputPassword();
        }
        
        public void OnInputFieldValueChangedMailAddress(string input)
        {
            mailAddressInputField.SetTextWithoutNotify(input);
            applyMailAddressButton.interactable = !string.IsNullOrEmpty(mailAddressInputField.text);
        }

        public void OnInputFieldValueChangeAuthCode(string input)
        {
            authCodeInputField.SetTextWithoutNotify(input);
            authCodeTransferButton.interactable = !string.IsNullOrEmpty(authCodeInputField.text);
        }
        #endregion

        public void OnClickCopy()
        {
            GUIUtility.systemCopyBuffer = loginIdText.text;
            string message = StringValueAssetLoader.Instance["menu.transfer.login_id_copyed"];
            notificationUI.ShowNotification(message);
        }

        public async void OnClickSavePassword()
        {
            await UserChangeTransferPasswordAPI();
            loginIdRoot.SetActive(true);
            closeButton.SetActive(true);
            backButton.gameObject.SetActive(false);
            inputPassRoot.SetActive(false);
        }

        public void OnClickChangePassword()
        {
            OnInputFieldValueChanged("");
            OnAgainInputFieldValueChanged("");
            inputWarningText.text = "";
            closeButton.SetActive(false);
            backButton.gameObject.SetActive(true);
            loginIdRoot.SetActive(false);
            inputPassRoot.SetActive(true);
        }

        public void OnClickShowTransferFlow()
        {
            TransferFlowModalWindow.Open(new TransferFlowModalWindow.WindowParams());
        }

        public void OnClickSetMailAddressButton()
        {
            // メールアドレス仮登録用オブジェクト表示
            OnInputFieldValueChangedMailAddress(String.Empty);
            loginIdRoot.SetActive(false);
            inputPassRoot.SetActive(false);
            inputMailAddressRoot.SetActive(true);
            // 戻るボタン表示
            backButton.gameObject.SetActive(true);
            closeButton.SetActive(false);
        }

        public async void OnClickApplyMailAddressButton()
        {
            await UserChangeTransferMailAddressAPI();
            // メールアドレスの仮登録が完了したら各種切り替え
            inputMailAddressRoot.SetActive(false);
            displayMailAddressTMP.text = mailAddressInputField.text;
            OnInputFieldValueChangeAuthCode("");
            inputAuthMailRoot.SetActive(true);
        }

        public async void OnClickAuthCodeReSendButton()
        {
            await UserChangeTransferMailAddressAPI();
            // 認証コードが再送信されたらモーダルを表示
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["transfer.authcode.resend_title"],
                StringValueAssetLoader.Instance["transfer.authcode.resend_message"],
                StringValueAssetLoader.Instance["transfer.authcode.resend_caution"], 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (v) => { v.Close(); }));
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            authCodeReSendButton.SetClickIntervalTimer(authCodeReSendButton.ClickTriggerInterval);
        }

        public async void OnClickAuthTransferButton()
        {
            await AuthTransferMailAPI();
            // メールアドレス認証完了
            inputAuthMailRoot.SetActive(false);
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["transfer.authcode.complete_title"],
                StringValueAssetLoader.Instance["transfer.authcode.complete_message"],
                null, 
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (v) => { v.Close(); }));
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            registerButton.SetActive(false);
            changeButton.SetActive(true);
            registeredMailAddress.SetActive(true);
            unregisteredMailAddress.SetActive(false);
            backButton.gameObject.SetActive(false);
            closeButton.SetActive(true);
            displayMailAddress.text = mailAddressInputField.text;
            displayMailAddress.gameObject.SetActive(true);
            linkMailAddress.SetActive(false);
            linkedMailAddress.SetActive(true);
            loginIdRoot.SetActive(true);
        }

        public void OnClickBackButton()
        {
            // 戻るを押した際に表示されている親オブジェクトから判断
            if (inputMailAddressRoot.activeSelf || inputPassRoot.activeSelf)
            {
                backButton.gameObject.SetActive(false);
                closeButton.SetActive(true);
                inputMailAddressRoot.SetActive(false);
                inputPassRoot.SetActive(false);
                loginIdRoot.SetActive(true);
            }
            else if(inputAuthMailRoot.activeSelf)
            {
                OnInputFieldValueChangedMailAddress(String.Empty);
                inputAuthMailRoot.SetActive(false);
                inputMailAddressRoot.SetActive(true);
            }
        }
        
        #region API
        
        private async UniTask<UserGetTransferSettingAPIResponse> UserGetTransferSettingAPI()
        {
            UserGetTransferSettingAPIRequest request = new UserGetTransferSettingAPIRequest();
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        private async UniTask UserChangeTransferPasswordAPI()
        {
            UserChangeTransferPasswordAPIRequest request = new UserChangeTransferPasswordAPIRequest();
            UserChangeTransferPasswordAPIPost post = new UserChangeTransferPasswordAPIPost { newPassword = passwordInputField.text };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }
        
        private async UniTask UserChangeTransferMailAddressAPI()
        {
            UserRegisterTransferMailTmpAPIRequest request = new UserRegisterTransferMailTmpAPIRequest();
            UserRegisterTransferMailTmpAPIPost post = new UserRegisterTransferMailTmpAPIPost() { mailAddress = mailAddressInputField.text };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }
        
        private async UniTask AuthTransferMailAPI()
        {
            UserAuthTransferMailAPIRequest request = new UserAuthTransferMailAPIRequest();
            UserAuthTransferMailAPIPost post = new UserAuthTransferMailAPIPost() { authCode = authCodeInputField.text };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }
        #endregion

        #region Other

        private void CheckInputPassword()
        {
            bool inputEmpty = string.IsNullOrEmpty(passwordInputField.text);
            bool againEmpty = string.IsNullOrEmpty(passwordAgainInputField.text);
            if (inputEmpty && againEmpty)
            {
                inputWarningText.text = "";
                applyButton.interactable = false; 
            }
            else if ((inputEmpty || againEmpty))
            {
                inputWarningText.text = StringValueAssetLoader.Instance["title.user_transfer_password_placeholder"];
                applyButton.interactable = false;
            }
            else if (passwordInputField.text　!= passwordAgainInputField.text)
            {
                inputWarningText.text = StringValueAssetLoader.Instance["menu.transfer.pass_not_right"];
                applyButton.interactable = false;
            }
            else if (passwordInputField.text.Length < 5)
            {
                inputWarningText.text = StringValueAssetLoader.Instance["menu.transfer.pass_count_limit"];
                applyButton.interactable = false;
            }
            else
            {
                inputWarningText.text = "";
                applyButton.interactable = true; 
            }

        }

        #endregion
    }
}