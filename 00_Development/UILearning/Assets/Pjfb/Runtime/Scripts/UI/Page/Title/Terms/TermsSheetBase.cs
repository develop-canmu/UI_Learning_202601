using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using Pjfb.CustomHtml;
using UnityEngine;

namespace Pjfb.Menu
{
    public class TermsSheetBase : Sheet
    {
        [SerializeField]
        private CustomHtmlObjectListContainer customHtmlObjectListContainer = null;

        public void SetView(string value, TermsModal.DisplayType displayType)
        {
            CustomHtmlObjectListContainer.ContainerParams data = new CustomHtmlObjectListContainer.ContainerParams();
            data.htmlBody = value;
            customHtmlObjectListContainer.SetDisplay(data);
        }
    }
}
