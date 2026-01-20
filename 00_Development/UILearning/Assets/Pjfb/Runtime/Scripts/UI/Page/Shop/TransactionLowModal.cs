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
using Pjfb.CustomHtml;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class TransactionLowModal : ModalWindow
    {
        [SerializeField]
        private CustomHtmlObjectListContainer customHtmlObjectListContainer = null;
 
        public static void Open()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TransactionLow, null);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            TermsGetTransactionLawAPIRequest request = new TermsGetTransactionLawAPIRequest();
            await APIManager.Instance.Connect(request);
            TermsGetTransactionLawAPIResponse response = request.GetResponseData();
            var data = new CustomHtmlObjectListContainer.ContainerParams();
            data.htmlBody = response.transactionLaw;
            customHtmlObjectListContainer.SetDisplay(data);
            await base.OnPreOpen(args, token);
        }
        
    }
}