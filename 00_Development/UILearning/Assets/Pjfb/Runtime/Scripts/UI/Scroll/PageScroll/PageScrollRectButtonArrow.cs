using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class PageScrollRectButtonArrow : MonoBehaviour
    {
        [SerializeField] PageScrollRect pageScrollRect = null;
        [SerializeField] private UIButton prevButton = null;
        [SerializeField] private UIButton nextButton = null;
        [SerializeField] private int movePage = 1;
        [SerializeField] private SE se = SE.None;
        
        private void Start()
        {
            SetActiveArrow();
            pageScrollRect.OnChangedPage += SetActiveArrow;
        }
        
        private void OnDestroy()
        {
            pageScrollRect.OnChangedPage -= SetActiveArrow;
        }
        
        public void OnClickPrev()
        {
            pageScrollRect.ScrollPage(-movePage, se);
            SetActiveArrow();
        }
        
        public void OnClickNext()
        {
            pageScrollRect.ScrollPage(movePage, se);
            SetActiveArrow();
        }

        // 矢印の表示切り替え
        private void SetActiveArrow()
        {
            prevButton.gameObject.SetActive(pageScrollRect.CurrentPage > 0);
            nextButton.gameObject.SetActive(pageScrollRect.CurrentPage < pageScrollRect.PageCount - 1);
        }
        
    }
}