using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class TermsModalData
    {
        private string terms = string.Empty;
        /// <summary>利用規約</summary>
        public string Terms { get { return terms; } }
        
        private string privacyPolicy = string.Empty;
        /// <summary>プライバシーポリシー</summary>
        public string PrivacyPolicy { get { return privacyPolicy; } }
        
        private string paymentLaw = string.Empty;
        /// <summary>資金決済法に基づく表示</summary>
        public string PaymentLaw { get { return paymentLaw; } }
        
        private TermsModal.DisplayType displayType = TermsModal.DisplayType.AskForAgree;
        /// <summary>表示形式</summary>
        public TermsModal.DisplayType DisplayType { get { return displayType; } }

        public TermsModalData(TermsGetTermsAPIResponse response, TermsModal.DisplayType displayType)
        {
            terms = response.serviceTerms;
            privacyPolicy = response.privacyPolicy;
            paymentLaw = response.paymentLaw;
            this.displayType = displayType;
        }
    }
    
    public class TermsModal : ModalWindow
    {
        public enum DisplayType
        {
            AskForAgree,
            Confirm
        }
        
        [SerializeField]
        private TermsSheetTerms termsSheet = null;
        
        [SerializeField]
        private TermsSheetPrivacyPolicy privacyPolicySheet = null;
        
        [SerializeField]
        private TermsSheetPaymentLaw paymentLawSheet = null;
        
        [SerializeField]
        private UIButton positiveButton = null;
        
        [SerializeField]
        private TextMeshProUGUI negativeButtonText = null;
        
        [SerializeField]
        private TextMeshProUGUI titleText = null;
        
        [SerializeField]
        private UIToggle checkbox = null;

        [SerializeField] 
        private GameObject checkboxRoot;
        
        public static async UniTask<bool> OpenAsync(DisplayType displayType, CancellationToken token)
        {
            // 同意が必要か
            if(displayType == DisplayType.AskForAgree)
            {
                // 同意済みか
                if(LocalSaveManager.saveData.isTermsAgreed)
                {
                    return true;
                }
            }
            
            TermsGetTermsAPIRequest request = new TermsGetTermsAPIRequest();
            TermsGetTermsAPIPost post = new TermsGetTermsAPIPost();
            post.appToken = null; 
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            TermsGetTermsAPIResponse response = request.GetResponseData();

            // モーダルを開く
            TermsModalData data = new TermsModalData(response, displayType);
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Terms, data, token);
            // 閉じるまで待機
            bool agreed = (bool)await modalWindow.WaitCloseAsync();
            return agreed;
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TermsModalData data  = (TermsModalData)args;
            // 表示形式
            DisplayType displayType = data.DisplayType;
            // データセット
            termsSheet.SetView(data.Terms, displayType);
            privacyPolicySheet.SetView(data.PrivacyPolicy, displayType);
            paymentLawSheet.SetView(data.PaymentLaw, displayType);
            // 同意ボタンの表示
            positiveButton.gameObject.SetActive(displayType == DisplayType.AskForAgree);
            positiveButton.interactable = checkbox.isOn;
            // ボタンテキスト変更
            negativeButtonText.text = displayType == DisplayType.AskForAgree ? StringValueAssetLoader.Instance["title.disagree"] : StringValueAssetLoader.Instance["common.close"];
            // タイトルテキスト変更
            titleText.text =  displayType == DisplayType.AskForAgree ? StringValueAssetLoader.Instance["title.terms"] : StringValueAssetLoader.Instance["menu.terms_button_text"];
            checkboxRoot.SetActive(displayType == DisplayType.AskForAgree);
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickPositiveButton()
        {
            SetCloseParameter(true);
            Close();
        }
        
        /// <summary>uGUI</summary>
        public void OnClickNegativeButton()
        {
            SetCloseParameter(false);
            Close();
        }
        
        /// <summary>uGUI</summary>
        public void OnClickCheckbox()
        {
            positiveButton.interactable = checkbox.isOn;
        }
    }
}
