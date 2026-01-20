using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.News
{
    public class NewsModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public NewsGetArticleListAPIResponse responseData;
            public string initialArticleUrl;
            public bool isDisableTodayAutomaticShow;
            public Action<bool> onChangedNoRepeatToggle;
            public Action onClosed;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private GameObject disableAutomaticShowToggle;
        [SerializeField] private ToggleContainer toggleContainer;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private NewsDetailView detailView;
        #endregion

        #region StaticInstance
        public static NewsModalWindow Instance;
        #endregion
        
        #region PrivateFields
        private WindowParams _windowParams;
        #endregion

        #region StaticMethods
        public static void Open(WindowParams windowParams)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.News, args: windowParams);
        }
        #endregion

        #region PublicMethods
        public void TryOpenArticle(string url)
        {
            var anArticle = _windowParams.responseData.articleList.ToList().Find(aData => aData.url.ToString() == url);
            if (anArticle != null)
            {
                OpenDetailView(anArticle);
            }
            else
            {
                Logger.LogError($"NewsModalWindow.TryOpenArticle failed to open an article. id:{url}");
            }
        }
        #endregion
        
        #region OverrideMethods
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Instance = this;
            _windowParams = (WindowParams) args;
            disableAutomaticShowToggle.SetActive(false);
            detailView.SetActive(false);
            return base.OnPreOpen(args, token);
        }

        protected override UniTask OnPreClose(CancellationToken token)
        {
            poolListContainer.SlideOut().Forget();
            return base.OnPreClose(token);
        }
        
        protected override UniTask OnOpen(CancellationToken token)
        {
            InitDisplay(_windowParams);
            return base.OnOpen(token);
        }
        #endregion

        #region PrivateMethods
        private void InitDisplay(WindowParams windowParams)
        {
            _windowParams = windowParams;
            disableAutomaticShowToggle.SetActive(windowParams.isDisableTodayAutomaticShow);
            detailView.Init();
            var initialIndex = 0;
            toggleContainer.Init(initialIndexDisplay: initialIndex, onSelectIndex: OnClickTab);
            OnClickTab(initialIndex).Forget();
            if (!string.IsNullOrEmpty(windowParams.initialArticleUrl))
            {
                TryOpenArticle(windowParams.initialArticleUrl);
                windowParams.initialArticleUrl = string.Empty;
            }
        }

        private void OpenDetailView(NewsArticle articleData)
        {
            detailView.SetDisplay(new NewsDetailView.ViewParams{articleData = articleData});
        }
        #endregion
        
#if UNITY_EDITOR || UNITY_ANDROID
        #region BackKey処理
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && AppManager.Instance.UIManager.ModalManager.GetTopModalWindow() == this)
            {
                SEManager.PlaySEAsync(SE.se_common_icon_tap).Forget();
                if (detailView.isActiveAndEnabled) detailView.OnClickBackButton();
                else OnClickClose();
            }
        }
        #endregion
#endif // UNITY_EDITOR || UNITY_ANDROID

        #region EventListeners
        public void OnClickClose()
        {
            if (poolListContainer.isAnimating) return;
            detailView.ClearStack();
            Close(onCompleted: _windowParams?.onClosed);
        }

        /// <summary>
        /// 「今日は表示しない」チェックボックス
        /// </summary>
        public void OnClickDisableTodayAutomaticShowToggle()
        {
            _windowParams.isDisableTodayAutomaticShow = !_windowParams.isDisableTodayAutomaticShow;
            disableAutomaticShowToggle.SetActive(_windowParams.isDisableTodayAutomaticShow);
            _windowParams.onChangedNoRepeatToggle.Invoke(_windowParams.isDisableTodayAutomaticShow);
        }
        
        private void OnClickNewsPoolItem(NewsPoolListItem.ItemParams itemParams)
        {
            Debug.Log($"OnClickNewsItem url:{itemParams.articleData.url}");
            OpenDetailView(itemParams.articleData);
        }

        private async UniTask OnClickTab(int index)
        {
            await poolListContainer.SlideOut();
            await poolListContainer.SetDataList(NewsManager.CreateNewsPoolListItemParams(_windowParams.responseData, (TabDefinition) index, OnClickNewsPoolItem), 1f, false);
        }
        #endregion
    }
}
