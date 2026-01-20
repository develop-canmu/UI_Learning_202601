using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    public class PoolListContainer : MonoBehaviour {
        #region SerializeFields
        [SerializeField] private RectTransform contentRectTransform;
        [SerializeField] private RectTransform maskRectTransform;
        [SerializeField] private PoolListItemBase itemPrefab;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private CanvasGroup scrollCanvas;
        [SerializeField] private CanvasGroup itemParentCanvas;
        [SerializeField] private int verticalSpacing = 15;
        [SerializeField] private int topMargin = 15;
        [SerializeField] private int bottomMargin = 0;
        [SerializeField] private int slideMaxDurationMilliSecond = 75;
        [SerializeField] private int itemSlideIntervalMaxDurationMilliSecond = 30;
        [SerializeField] private int itemSlideDurationMilliSecond = 100;
        #endregion

        #region Properties
        public List<PoolListItemBase.ItemParamsBase> ListItemParams { get; private set; } = new();
        private List<PoolListItemBase> pooledListItems = new();
        private const int SlidePositionDelta = -50;
        private const float FadeInDuration = 0.25f;
        private const float PoolTopAdjustmentValue = 0.3f;
        private const float PoolBotAdjustmentValue = 1.3f;
        public bool isAnimating { get; private set; }
        public float scrollValue => scrollRect.verticalScrollbar.value;
        
        private bool _disableSlideAnimation => ListItemParams.IsNullOrEmpty();
        private CancellationToken _cancellationTokenOnDestroy;
        public ScrollRect ScrollRect { get { return scrollRect; } }
        private bool setDerty = false;  // ビュー更新用フラグ
        #endregion

        #region PublicMethods
        public void Clear()
        {
            Reset();
        }
        
        public async UniTask SetDataList<T>(IEnumerable<T> itemParams, float scrollValue = 1f, bool slideIn = true) where T:PoolListItemBase.ItemParamsBase
        {
            Reset();
            ListItemParams = itemParams.Select(anItem => (PoolListItemBase.ItemParamsBase)anItem).ToList();
            SetListItemPositionHeight();
            await SetScrollValue(scrollValue);
            if (slideIn) await SlideIn();
        }

        public int GetTotalHeight()
        {
            if (!ListItemParams.Any()) return 0;
            
            var lastItem = ListItemParams.Last();
            return -lastItem.itemPosition + lastItem.itemHeight;
        }

        public void SetDirtyFlg()
        {
            setDerty = true;
        }
        
        #endregion

        #region SlideProcess
        [ContextMenu("SlideIn")] public async UniTask SlideIn() => await SlideIn(onComplete: null);
        [ContextMenu("SlideOut")] public async UniTask SlideOut() => await SlideOut(onComplete: null);
        
        public async UniTask SlideIn(Action onComplete)
        {
            if (_disableSlideAnimation) {
                onComplete?.Invoke();
                return;
            }
            
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            isAnimating = true;
            if (scrollCanvas != null)
            {
                scrollCanvas.alpha = 0;
            }
            itemParentCanvas.blocksRaycasts = false;
            var activeListItemParams = GetOrderedActiveListItemParams();
            var itemSlideInterval = (int)MathF.Min(itemSlideIntervalMaxDurationMilliSecond, (float)slideMaxDurationMilliSecond / activeListItemParams.Count);
            var itemSlideDurationSecond = (float)itemSlideDurationMilliSecond / 1000;
            activeListItemParams.ForEach(aData => aData.nullableActiveListItem.canvasGroup.alpha = 0);
            foreach (var activeListItemParam in activeListItemParams)
            {
                await Task.Delay(millisecondsDelay: itemSlideInterval, cancellationToken: _cancellationTokenOnDestroy);
                var activeListItem = activeListItemParam.nullableActiveListItem;
                var startPosition = activeListItemParam.itemPosition + SlidePositionDelta;
                var endPosition = activeListItemParam.itemPosition;
                _ = DOTween.To(lerpValue => {
                    activeListItem.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(startPosition, endPosition, lerpValue));
                    activeListItem.canvasGroup.alpha = lerpValue * 2;
                }, 0, 1, itemSlideDurationSecond);
            }
            
            await DOTween.To(val =>
            {
                if (scrollCanvas != null)
                {
                    scrollCanvas.alpha = val;
                }
            }, 0, 1, FadeInDuration);
            if (_cancellationTokenOnDestroy.IsCancellationRequested) return;
            
            isAnimating = false;
            itemParentCanvas.blocksRaycasts = true;
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            onComplete?.Invoke();
        }
        
        public async UniTask SlideOut(Action onComplete)
        {
            if (_disableSlideAnimation) {
                onComplete?.Invoke();
                return;
            }
            
            isAnimating = true;
            itemParentCanvas.blocksRaycasts = false;
            if (scrollCanvas != null)
            {
                scrollCanvas.alpha = 1;
            }
            var activeListItemParams = GetOrderedActiveListItemParams();
            var itemSlideInterval = (int)MathF.Min(itemSlideIntervalMaxDurationMilliSecond, (float)slideMaxDurationMilliSecond / activeListItemParams.Count);
            var itemSlideDurationSecond = (float)itemSlideDurationMilliSecond / 1000;
            activeListItemParams.ForEach(aData => aData.nullableActiveListItem.canvasGroup.alpha = 1);
            foreach (var activeListItemParam in activeListItemParams)
            {
                await Task.Delay(millisecondsDelay: itemSlideInterval, cancellationToken: _cancellationTokenOnDestroy);
                var activeListItem = activeListItemParam.nullableActiveListItem;
                var startPosition = activeListItemParam.itemPosition;
                var endPosition = activeListItemParam.itemPosition + SlidePositionDelta;
                _ = DOTween.To(lerpValue => {
                    activeListItem.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(startPosition, endPosition, lerpValue));
                    activeListItem.canvasGroup.alpha = 1 - lerpValue;
                }, 0, 1, itemSlideDurationSecond);
            }
            
            await DOTween.To(val =>
            {
                if (scrollCanvas != null)
                {
                    scrollCanvas.alpha = val;
                }
            }, 1, 0, FadeInDuration);
            if (_cancellationTokenOnDestroy.IsCancellationRequested) return;
            
            itemParentCanvas.blocksRaycasts = true;
            isAnimating = false;
            onComplete?.Invoke();
        }

        public void RefreshView()
        {
            Vector3 previousPos = contentRectTransform.localPosition;
            ListItemParams.Where(anItem => anItem.nullableActiveListItem != null).ToList().ForEach(AddPool);
            pooledListItems.ForEach(anItem => anItem.gameObject.SetActive(false));
            SetListItemPositionHeight();
            contentRectTransform.localPosition = previousPos;
            OnScrollValueChanged(Vector2.zero);
        }

        #endregion

        #region OverrideMethods
        private void Awake()
        {
            itemPrefab.gameObject.SetActive(false);
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            _cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
        }

        private void OnScrollValueChanged(Vector2 v)
        {
            if (scrollCanvas != null)
            {
                scrollCanvas.alpha = 1;
            }
            var currentYPosition = contentRectTransform.anchoredPosition.y;
            var viewportHeight = maskRectTransform.rect.y;
            var poolTopViewport = -viewportHeight * PoolTopAdjustmentValue;
            var poolBotViewport = viewportHeight * PoolBotAdjustmentValue;
            ListItemParams.ForEach(anItemParam =>
            {
                var topPosition = anItemParam.itemPosition + currentYPosition;
                var botPosition = topPosition - anItemParam.itemHeight;
                if (poolTopViewport > botPosition && topPosition > poolBotViewport)
                {
                    if (anItemParam.nullableActiveListItem != null) return;
                    anItemParam.nullableActiveListItem = pooledListItems.Any()
                        ? pooledListItems.PopLast()
                        : Instantiate(itemPrefab, parent: contentRectTransform);
                    anItemParam.nullableActiveListItem.rectTransform.anchoredPosition = new Vector2(0, anItemParam.itemPosition);
                    anItemParam.nullableActiveListItem.gameObject.SetActive(true);
                    anItemParam.nullableActiveListItem.canvasGroup.alpha = 1;
                    anItemParam.nullableActiveListItem.Init(anItemParam);
                }
                else
                {
                    if (anItemParam.nullableActiveListItem == null) return;
                    AddPool(anItemParam);
                }
            });
        }

        private void LateUpdate()
        {
            if(!setDerty) return;
            CalculateLayout();
            // フラグの切り替え
            setDerty = false;
        }

        #endregion
        
        #region PrivateMethods
        
        private void SetListItemPositionHeight()
        {
            var prefabHeight = (int)itemPrefab.rectTransform.sizeDelta.y;
            var itemPosition = -topMargin;
            ListItemParams.ForEach(anItem =>
            {
                anItem.SetItemPositionHeight(
                    itemPosition: itemPosition,
                    itemHeight: anItem.GetItemHeight(itemPrefab, prefabHeight));
                itemPosition -= anItem.itemHeight + verticalSpacing;
            });
            var totalHeight = -itemPosition;
            contentRectTransform.pivot = new Vector2(0.5f, 1); // middle top
            contentRectTransform.localPosition = Vector3.zero; // scrollValue:0状態
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight + bottomMargin);
        }
        
        private void Reset()
        {
            ListItemParams.Where(anItem => anItem.nullableActiveListItem != null).ToList().ForEach(AddPool);
            ListItemParams.Clear();
            pooledListItems.ForEach(anItem => anItem.gameObject.SetActive(false));
        }

        private void AddPool(PoolListItemBase.ItemParamsBase itemParam)
        {
            itemParam.nullableActiveListItem.Pooled();
            itemParam.nullableActiveListItem.gameObject.SetActive(false);
            pooledListItems.Add(itemParam.nullableActiveListItem);
            itemParam.nullableActiveListItem = null;
        }

        private List<PoolListItemBase.ItemParamsBase> GetOrderedActiveListItemParams()
        {
            return ListItemParams
                .Where(anItem => anItem.nullableActiveListItem != null)
                .OrderByDescending(anItem => anItem.itemPosition).ToList();
        }
        
        /// <summary>
        /// 注意：スクロールバリュー設定は1フレームを待たないといけない場合があるため、1フレームの間は非表示にする
        /// 調査用：フレームがずらさないと<see cref="NewsModalWindow.Init"/> で行なったpoolListContainer.SetDataListのscrollValue設定が無効になる
        /// </summary>
        private async UniTask SetScrollValue(float scrollValue)
        {
            scrollRect.StopMovement();
            itemParentCanvas.alpha = 0;
            await UniTask.DelayFrame(1, cancellationToken: _cancellationTokenOnDestroy);

            itemParentCanvas.alpha = 1;
            if (scrollRect.verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar.value = scrollValue;
            }
            OnScrollValueChanged(Vector2.zero);
        }
                
        // 画像が更新された場合、再度サイズ調整を行う
        private void CalculateLayout()
        {
            var prefabHeight = (int)itemPrefab.rectTransform.sizeDelta.y;
            var itemPosition = -topMargin;
            var currentYPosition = contentRectTransform.anchoredPosition.y;
            var viewportHeight = maskRectTransform.rect.y;
            var poolTopViewport = -viewportHeight * PoolTopAdjustmentValue;
            var poolBotViewport = viewportHeight * PoolBotAdjustmentValue;
            
            ListItemParams.ForEach(anItem =>
            {
                // 高さを再取得しそれに合わせて位置を調整
                anItem.SetItemPositionHeight(
                    itemPosition: itemPosition,
                    itemHeight: anItem.GetItemHeight(itemPrefab, prefabHeight));
                itemPosition -= anItem.itemHeight + verticalSpacing;
                var topPosition = anItem.itemPosition + currentYPosition;
                var botPosition = topPosition - anItem.itemHeight;
                // 描画範囲内にいるか判定してオブジェクトの座標を更新
                if (anItem.nullableActiveListItem == null) return;
                if (poolTopViewport > botPosition && topPosition > poolBotViewport)
                {
                    anItem.nullableActiveListItem.rectTransform.anchoredPosition = new Vector2(0, anItem.itemPosition);
                }
                else
                {
                    // 高さ変更によって描画範囲から外れてしまったオブジェクトを外す
                    AddPool(anItem);
                }
            });
            // 全体の大きさを見てContentの大きさを変更
            var totalHeight = -itemPosition;
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight + bottomMargin);
        }
        
        #endregion

        private void OnValidate()
        {
            SetListItemPositionHeight();
        }
    }
}