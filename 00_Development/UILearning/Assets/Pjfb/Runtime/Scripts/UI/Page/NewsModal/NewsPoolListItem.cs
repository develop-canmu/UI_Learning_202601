using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.News
{
    public class NewsPoolListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public const int FrameSmallSize = 220;  // 画像なし
            private int frameSize = FrameSmallSize; // 画像適用後のギャップを最小限にするために画像なしの値で初期化

            public int FrameSize
            {
                get
                {
                    return frameSize;
                }
                set
                {
                    frameSize = value;
                }
            }
            
            public bool isDetailPage;
            public NewsArticle articleData;
            public Action<ItemParams> onClickItem;

            public NewsCategories categoryEnum { get; }
            
            public ItemParams(bool isDetailPage, NewsArticle articleData, Action<ItemParams> onClickItem)
            {
                this.isDetailPage = isDetailPage;
                this.articleData = articleData;
                this.onClickItem = onClickItem;
                
                this.categoryEnum = int.TryParse(articleData.category, out var result) ? (NewsCategories)result : NewsCategories.None;
            }

            public override int GetItemHeight(PoolListItemBase poolListItemBase, int prefabHeight)
            {
                return isDetailPage || string.IsNullOrEmpty(articleData.imagePath) ? FrameSmallSize : frameSize;
            }
        }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text typeText;
        [SerializeField] private TMPro.TMP_Text dateText;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private CancellableWebTexture bottomBannerImage;
        [SerializeField] private RawImage bannerImageTexture;
        [SerializeField] private RectTransform bannerTransform;
        [SerializeField] private RectTransform bottomGameObject;
        [SerializeField] private RectTransform contentsTransform;
        [SerializeField] private NewsCategoryLogoImage logoImage;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private int margin = 5; // 間隔
        #endregion

        #region PrivateFields
        private ItemParams itemParams;

        private int frameSize;  // 画像に併せて大きさを変更

        #endregion

        #region OverrideMethods
        public override void Init(ItemParamsBase itemParams)
        {
            this.itemParams = (ItemParams)itemParams;
            UpdateDisplay(this.itemParams).Forget();
            base.Init(itemParams);
        }
        #endregion
    
        #region PrivateMethods
        private async UniTask UpdateDisplay(ItemParams itemParams)
        {
            var articleData = itemParams.articleData;
            typeText.text = NewsManager.CategoryNameDictionary.GetValueOrDefault(articleData.category);
            dateText.text = articleData.startAt.TryConvertToDateTime().GetNewsDateTimeString();
            descriptionText.text = articleData.title;
            logoImage.SetTexture(itemParams.categoryEnum);
            if (itemParams.isDetailPage || string.IsNullOrEmpty(itemParams.articleData.imagePath))
            {
                bottomGameObject.gameObject.SetActive(false);
                contentsTransform.sizeDelta = new Vector2(contentsTransform.sizeDelta.x, ItemParams.FrameSmallSize);
            }
            else
            {
                bottomGameObject.gameObject.SetActive(true);
                await bottomBannerImage.SetTextureAsync($"{AppEnvironment.AssetBrowserURL}/{itemParams.articleData.imagePath}");
                // 画像表示に必要なサイズを計算
                frameSize = bannerImageTexture.texture.height + margin;
                // オブジェクトの大きさを変更
                bottomGameObject.sizeDelta = new Vector2(bottomGameObject.sizeDelta.x, frameSize);
                bannerTransform.sizeDelta = new Vector2(bannerTransform.sizeDelta.x, frameSize);
                itemParams.FrameSize = frameSize + ItemParams.FrameSmallSize;
                contentsTransform.sizeDelta = new Vector2(contentsTransform.sizeDelta.x, itemParams.FrameSize);
                // 画像が適用されたことを知らせる
                poolListContainer.SetDirtyFlg();
            }
        }
        #endregion
        
        #region EventListeners
        public void OnClickItem()
        {
            itemParams?.onClickItem?.Invoke(itemParams);
        }
        #endregion
    }
}