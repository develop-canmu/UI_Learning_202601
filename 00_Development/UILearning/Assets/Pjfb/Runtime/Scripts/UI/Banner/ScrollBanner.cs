using System;
using System.Collections;
using CruFramework.UI;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb
{
    public class ScrollBanner : MonoBehaviour
    {
        
        [SerializeField]
        private SE slideSe = SE.se_common_slide;
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        public ScrollGrid ScrollGrid { get { return scrollGrid;} }

        [SerializeField]
        private ScrollBannerPage pagePrefab = null;
        
        [SerializeField]
        private Transform pageParent = null;
        
        [SerializeField]
        private int offsetPosition = -1;
        
        [SerializeField]
        private float focusedScale = 1.0f;

        [SerializeField]
        private ScrollBannerLayoutAdjuster nullableLayoutAdjuster;
        
        private IList bannerDatas = null;
        private ScrollBannerPage[] pages = null;
        
        private ScrollGridItem focusedBanner = null;
        public event Action<int> onChangedPage;
        // 現在ページ
        private int currentIndex = 0;

        private void Awake()
        {
            // ページ変更通知
            scrollGrid.OnChangedPage += OnChangedPage;
            // スライド開始時
            scrollGrid.OnBeginSlideAnimation += OnBeginSlide;
            // リスト位置調整
            scrollGrid.ItemListOffset = offsetPosition;
        }

        private void OnDestroy()
        {
            // ページ変更通知
            scrollGrid.OnChangedPage -= OnChangedPage;
            // スライド開始時
            scrollGrid.OnBeginSlideAnimation -= OnBeginSlide;
        }

        private void OnBeginSlide()
        {
            if(slideSe != SE.None)
            {
                SEManager.PlaySE(slideSe);
            }
        }

        /// <summary>ページ変更通知</summary>
        private void OnChangedPage(int index)
        {
            if(bannerDatas == null || bannerDatas.Count <= 0) return;
            
            if(pages != null)
            {
                // 現在開いているページ
                pages[currentIndex].SetEnable(false);
            }
            // フォーカスしていたバナーのスケールをもとに戻す
            if(focusedBanner != null)
            {
                focusedBanner.transform.localScale = Vector3.one;
            }
            
            // 現在のページを更新
            currentIndex = index;
            if(pages != null)
            {
                pages[currentIndex].SetEnable(true);
            }
            focusedBanner = scrollGrid.GetItem(bannerDatas[currentIndex]);
            if (focusedBanner != null) focusedBanner.transform.localScale = Vector3.one * focusedScale;
            onChangedPage?.Invoke(currentIndex);
        }

        /// <summary>バナーデータセット</summary>
        public void SetBannerDatas(IList bannerDatas)
        {
            if(bannerDatas == null || bannerDatas.Count <= 0) return;
            
            currentIndex = 0;
            this.bannerDatas = bannerDatas;
            // スクロール設定
            scrollGrid.SetItems(bannerDatas);
            // ページが生成済みなら削除
            if(pages != null)
            {
                foreach (ScrollBannerPage page in pages)
                {
                    GameObject.Destroy(page.gameObject);
                }
            }
            
            if(pagePrefab != null && pageParent != null)
            {
                // ページ生成
                pages = new ScrollBannerPage[scrollGrid.PageCount];
                for (int i = 0; i < pages.Length; i++)
                {
                    pages[i] = GameObject.Instantiate(pagePrefab, pageParent);
                    pages[i].gameObject.SetActive(true);
                    pages[i].SetEnable(scrollGrid.CurrentPage == i);
                }
            }

            if (nullableLayoutAdjuster != null) 
            {
                nullableLayoutAdjuster.TryResetSpacing();
            }
            
            ScrollGridItem banner = scrollGrid.GetItem(bannerDatas[currentIndex]);
            banner.transform.localScale = Vector3.one * focusedScale;
        }

        public void SetIndex(int index, bool isAnimation = false)
        {
            if(bannerDatas == null || bannerDatas.Count <= 0) return;
            
            ScrollGrid.SetPage(index, isAnimation);
            OnChangedPage(index);
        }
        
        public object GetNullableBannerData() => bannerDatas != null && bannerDatas.Count > currentIndex ? bannerDatas[currentIndex] : null;
    }
}
