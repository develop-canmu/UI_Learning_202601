using System.Collections;
using System.Collections.Generic;
using Pjfb.Menu;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class InquiryModal : ModalWindow
    {
        [SerializeField]
        private TextMeshProUGUI opinionsAndRequestsButton = null;
        
        [SerializeField]
        private TextMeshProUGUI bugReportButtonText = null;
        
        /// <summary>uGUI</summary>
        public void OnClickBeforeInquiryButton()
        {
            string title = StringValueAssetLoader.Instance["title.inquiry_before_inquiry"];
            string body = StringValueAssetLoader.Instance["menu.inquiry.before_inquiry"];
            ArticleModalWindow.Open(new ArticleModalWindow.WindowParams
            {
                titleText = title,
                bodyText = body,
            });
        }
        
        /// <summary>uGUI</summary>
        public void OnClickOpinionsAndRequestsButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.InquiryInput, opinionsAndRequestsButton.text);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickBugReportButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.InquiryInput, bugReportButtonText.text);
        }
    }
}
