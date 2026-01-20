using System.Collections.Generic;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Home;
using Pjfb.News;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.DeepLink
{
    public class DeepLinkManager : SingletonMonoBehaviour<DeepLinkManager>
    {
        #region SerializeField
        [SerializeField] private UIButton homeButton;
        #endregion
        
        #region PublicProperties
        public DeepLinkDataContainer deepLinkDataContainer { get; private set; }
        #endregion

        #region PrivateProperties
        private CancellationTokenSource cancellationTokenSource = null;
        #endregion

        #region OverrideMethods
        protected override void OnAwake()
        {
            // 注意：androidは未検証になります
            // blue-lock-pwc://?<パラメーター> でOnDeepLinkActivatedを実行できるはず
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // absoluteUrl:
                // For Android, iOS, or Universal Windows Platform (UWP) this is a deep link URL. (Read Only)
                deepLinkDataContainer = new DeepLinkDataContainer (Application.absoluteURL);
            }
        }
        #endregion

        #region PublicMethods
        public void ClearDeepLinkCache()
        {
            deepLinkDataContainer = null;
        }
        #endregion

        #region EventListener
        private async void OnDeepLinkActivated(string url)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            
            Logger.Log($"DeepLinkManager.OnDeepLinkActivated url:{url}");
            deepLinkDataContainer = new DeepLinkDataContainer (url);
            
            if (deepLinkDataContainer.parameters.TryGetValue("openNews", out var openNewsUrl))
            {
                // お知らせ画面がすでに表示されている場合
                if (NewsModalWindow.Instance != null)
                {
                    NewsModalWindow.Instance.TryOpenArticle(openNewsUrl);
                    return;
                }

                var currentCancellationTokenSource = cancellationTokenSource = new CancellationTokenSource();
                await UniTask.WaitUntil(() => AppManager.Instance.UIManager.ModalManager.GetTopModalWindow() == null);
                if (currentCancellationTokenSource.IsCancellationRequested) return;

                // ホーム画面にいる状態かつ他のモーダルが表示されていない状態の場合
                if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Home)
                {
                    var homePage = AppManager.Instance.UIManager.PageManager.CurrentPageObject as HomePage;
                    if (homePage != null && homePage.CurrentPageObject is HomeTopPage)
                    {
                        HomeTopPage.Instance.TryShowNews(isFromTitle: false, nextAction: null);
                        return;
                    }
                } 
            
                // ホームボタンが押せる状態ならホームボタンのイベントでホーム画面に移動させる
                if (homeButton.TryClickWithRaycast()) return;
            }
        }
        #endregion

#if UNITY_EDITOR
        [ContextMenu("TestDeepLink")]
        public void TestDeepLink()
        {
            OnDeepLinkActivated("blue-lock-pwc://?openNews=news/update_221119_2");
        }
#endif
    }

    public class DeepLinkDataContainer
    {
        public string url;
        public Dictionary<string, string> parameters;

        public DeepLinkDataContainer(string url)
        {
            this.url = url;

            var splitUrl = url.Split("?");
            var parametersUrl = splitUrl.Length >= 2 ? splitUrl[1] : string.Empty;
            var splitParameters = parametersUrl.Split("&");
            if (splitParameters.Length == 0) parameters = null;
            else
            {
                parameters = new Dictionary<string, string>();
                foreach (var parameter in splitParameters)
                {
                    var keyValue = parameter.Split('=');
                    if (keyValue.Length == 2)
                    {
                        parameters.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
        }
    }
}
