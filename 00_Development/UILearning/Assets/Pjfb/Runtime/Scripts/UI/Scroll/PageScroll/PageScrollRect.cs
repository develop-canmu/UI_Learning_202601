using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pjfb
{
    public class PageScrollRect : ScrollRect
    {
        [SerializeField] private SE slideSe = SE.se_common_slide;
        [SerializeField] private RectTransform[] scrollItemList;
        [SerializeField] private ScrollBannerPage page = null;
        [SerializeField] private RectTransform pageParent = null;
        // ページスクロールにかかる時間
        [SerializeField] private float scrollTime = 0.1f;
        // ページが変わるまでに必要なスワイプ量
        [SerializeField] private float needSwipeAmount = 0.1f;
        // ページ変更通知
        public event Action OnChangedPage; 
        // 総ページ数
        public int PageCount{get{return scrollItemList.Length;}}
        // 現在のページ番号
        private int currentPage = 0;
        // 現在のページ数
        public int CurrentPage{get{return currentPage;}}
        // 1ページあたりのスクロール量
        private float pageScrollAmount = 0;
        // ドラッグ開始位置
        private float beginDragPos;
        // ドラッグ終了位置
        private float endDragPos;
        // ページ数表示
        private ScrollBannerPage[] pages;

        // スクロール実行時間
        private float scrollTimer = 0;
        // スクロール実行中か
        private bool isScrolling = false;
        
        public void InitScroll()
        {
            // ビューポートからスクロールビューのサイズを取得
            float width = viewport.rect.width * viewport.localScale.x;
            // コンテントをスクロールビューのサイズと表示する要素数に合わせる
            content.sizeDelta = new Vector2(width * scrollItemList.Length, content.sizeDelta.y);
            Vector3 pos = viewport.position;
            // 各要素の座標を設定
            foreach (RectTransform scrollItem in scrollItemList)
            {
                scrollItem.gameObject.SetActive(true);
                scrollItem.localPosition = pos;
                pos.x += width;
            }
            
            // 1ページあたりのスクロール量を計算
            if (scrollItemList.Length > 1)
            {
                pageScrollAmount = 1.0f / (scrollItemList.Length - 1);
            }
            else
            {
                pageScrollAmount = 1.0f;
            }

            if (page != null)
            {
                pages = new ScrollBannerPage[scrollItemList.Length];
                for (int i = 0; i < scrollItemList.Length; i++)
                {
                    pages[i] = Instantiate(page, pageParent);
                    pages[i].gameObject.SetActive(true);
                    pages[i].SetEnable(false);
                }
                pages[currentPage].SetEnable(true);
                page.gameObject.SetActive(false);
            }

        }

        // スワイプ開始
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if(scrollItemList.Length <= 1 || isScrolling) return;
            beginDragPos = horizontalNormalizedPosition;
            base.OnBeginDrag(eventData);
        }

        // スワイプ終了
        public override void OnEndDrag(PointerEventData eventData)
        {
            if(scrollItemList.Length <= 1 || isScrolling) return;
            // スワイプ量を計算
            endDragPos = horizontalNormalizedPosition;
            float moveAmount = horizontalNormalizedPosition - beginDragPos;
            
            int movePage = 0;
            // スワイプ量が一定以上ならページを移動
            if(moveAmount > needSwipeAmount)
            {
                movePage = 1;
            }
            else if(moveAmount < -needSwipeAmount)
            {
                movePage = -1;
            } 
            
            ScrollPage(movePage);
            base.OnEndDrag(eventData);
        }

        // ページ移動
        public void ScrollPage(int movePage, SE se = SE.None)
        {
            if(isScrolling) return;
            // 移動前のページ番号を取得
            int beforePage = currentPage;
            // ページ番号を更新
            currentPage += movePage;
            // ページ番号の範囲を制限
            if (currentPage >= scrollItemList.Length)
            {
                currentPage = scrollItemList.Length - 1;
            }
            else if (currentPage < 0)
            {
                currentPage = 0;
            }
         
            // スクロール時間を初期化
            scrollTimer = scrollTime;
            isScrolling = true;
            // ページ表示更新
            if (pages != null)
            {
                pages[beforePage].SetEnable(false);
                pages[currentPage].SetEnable(true);
            }
            // SE再生
            SE sePlay = se == SE.None ? slideSe : se;
            if(slideSe != SE.None)
            {
                SEManager.PlaySE(sePlay);
            }
            // ページ変更通知
            if(OnChangedPage != null)
            {
                OnChangedPage();
            }
        }

        private void ScrollAnimation()
        {
            // スクロール時間を減らす
            scrollTimer -= Time.deltaTime;
            // スクロール時間が0以下になったらスクロール終了
            if (scrollTimer <= 0)
            {
                // 補正しておく
                horizontalNormalizedPosition = currentPage * pageScrollAmount;
                endDragPos = horizontalNormalizedPosition;

                isScrolling = false;
            }
            else
            {
                // スクロール中の位置を計算
                float pos = Mathf.Lerp(endDragPos, currentPage * pageScrollAmount, 1 - scrollTimer / scrollTime);
                // スクロール
                horizontalNormalizedPosition = pos;
            }
        }
        
        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (isScrolling)
            {
                ScrollAnimation();
            }
        }
    }
}