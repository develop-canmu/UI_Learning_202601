using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.News
{
    public class NewsDetailHeader : MonoBehaviour
    {
        #region Parameters
        public class Parameters
        {
            public NewsArticle articleData;
        }
        #endregion
        
        #region SerializeFields
        // TODO: お知らせ詳細画面のデザインレイアウトが決まったらリファクタする
        [SerializeField] private NewsPoolListItem poolListItem;
        #endregion

        #region PrivateProperties
        private readonly NewsPoolListItem.ItemParams initItemParams = new (isDetailPage: false, articleData: new NewsArticle(), onClickItem: null);
        #endregion

        #region PublicMethods
        public void Init()
        {
            poolListItem.Init(initItemParams);
        }

        public void SetDisplay(Parameters parameters)
        {
            poolListItem.Init(new NewsPoolListItem.ItemParams(isDetailPage: true, articleData: parameters.articleData, onClickItem: null));
        }
        #endregion
    }
}
