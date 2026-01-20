using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class InquiryInputModal : ModalWindow
    {
        private const int MaxBodyCount = 999;
        
        [SerializeField]
        private TextMeshProUGUI inquiryTypeText = null;
        
        [SerializeField]
        private TMP_InputField mailAddressInput = null;
        
        [SerializeField]
        private TMP_InputField bodyInput = null;
        
        [SerializeField]
        private TextMeshProUGUI bodyCountText = null;
        
        [SerializeField]
        private TextMeshProUGUI showText = null;
        
        [SerializeField]
        private UIButton positiveButton = null;
        
        [SerializeField]
        private GameObject alertText = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // お問い合わせの種類
            inquiryTypeText.text = (string)args;
            // お問い合わせの入力数セット
            SetBodyCountText(bodyInput.text.Length);
            // 送信可能かチェック
            CheckPositiveButtonInteractable();
            //入力制限追加
            bodyInput.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, bodyInput.characterLimit,bodyInput.fontAsset);
            return base.OnPreOpen(args, token);
        }

        /// <summary>uGUI</summary>
        public void OnClickPositiveButton()
        {
            InquiryConfirmModalWindow.Open(new InquiryConfirmModalWindow.WindowParams
            {
                inquiryTypeText = inquiryTypeText.text,
                mailAddressText = mailAddressInput.text,
                bodyText = bodyInput.text
            });
        }
        
        /// <summary>お問い合わせの入力数をセット</summary>
        private void SetBodyCountText(int count)
        {
            bodyCountText.text = string.Format(StringValueAssetLoader.Instance["title.inquiry_body_count"], count, MaxBodyCount);
        }
        
        /// <summary>uGUI</summary>
        public void CheckPositiveButtonInteractable()
        {
            bool interactable = 
                !string.IsNullOrEmpty(mailAddressInput.text) &&
                !string.IsNullOrEmpty(bodyInput.text) &&
                bodyInput.text.Length <= MaxBodyCount &&
                !alertText.activeSelf;
                
            positiveButton.interactable = interactable;
        }
        
        /// <summary>uGUI</summary>
        public void OnEndEditText(string input)
        {
            input = StringUtility.GetLimitNumCharacter(input,bodyInput.characterLimit);
            bodyInput.SetTextWithoutNotify(input);
            SetBodyCountText(bodyInput.text.Length);
        }
        
        /// <summary>uGUI</summary>
        public void OnInputValueChanged(string input)
        {
            SetBodyCountText(input.Length);
            showText.text = input;
        }
        
        /// <summary>uGUI</summary>
        public void OnMailValueChanged(string input)
        {
            Regex mailReg = new Regex("^\\S+@\\S+\\.\\S+$");
            bool mailAvailable = mailAddressInput.text.Length == 0 || mailReg.IsMatch(input);
            alertText.SetActive(!mailAvailable);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickInputField()
        {
            bodyInput.ActivateInputField();
        }

    }
}
