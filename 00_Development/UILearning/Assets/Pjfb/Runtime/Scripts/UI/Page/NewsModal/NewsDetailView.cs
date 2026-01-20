using System.Collections.Generic;
using Pjfb.CustomHtml;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.News
{
    public class NewsDetailView : MonoBehaviour
    {
        #region Params
        public class ViewParams
        {
            public NewsArticle articleData;
        }
        #endregion
        
        #region SerializeField
        [SerializeField] private NewsDetailHeader header;
        [SerializeField] private CustomHtmlObjectListContainer customHtmlObjectListContainer;
        #endregion

        #region PrivateFields
        private Stack<ViewParams> openedViewParamsStack = new ();
        #endregion

        #region PublicMethods
        public void Init()
        {
            header.Init();
            if (openedViewParamsStack.Count > 0) SetDisplay(openedViewParamsStack.Peek(), isPushToStack: false);
            else gameObject.SetActive(false);
        }

        public void ClearStack()
        {
            openedViewParamsStack.Clear();
        }

        public void SetDisplay(ViewParams viewParams, bool isPushToStack = true)
        {
            if (isPushToStack) openedViewParamsStack.Push(viewParams);
            SetActive(true);
            header.SetDisplay(new NewsDetailHeader.Parameters{articleData = viewParams.articleData});
            customHtmlObjectListContainer.SetDisplay(new CustomHtmlObjectListContainer.ContainerParams
            {
                htmlBody = viewParams.articleData.body,
            });
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
        #endregion

        #region EventHandler
        public void OnClickBackButton()
        {
            openedViewParamsStack.Pop();
            if (openedViewParamsStack.Count > 0) SetDisplay(openedViewParamsStack.Peek(), isPushToStack: false);
            else gameObject.SetActive(false);
        }
        #endregion
    }
}