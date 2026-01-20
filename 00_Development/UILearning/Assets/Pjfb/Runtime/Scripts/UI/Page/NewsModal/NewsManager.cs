using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Pjfb.DeepLink;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;

namespace Pjfb.News
{
    public static class NewsManager
    {
        #region PlayerPrefs
        private const string NewsDisableTodayAutomaticShow = "NewsDisableTodayAutomaticShow";
        public static bool NewsDisableTodayAutomaticShowPlayerPrefs
        {
            get => ObscuredPrefs.Get<int>(key: NewsDisableTodayAutomaticShow, defaultValue: 0) == 1;
            set => ObscuredPrefs.Set<int>(key: NewsDisableTodayAutomaticShow, value ? 1 : 0);
        }
        private const string NewsLastShown = "NewsLastShown";
        public static DateTime NewsLastShownPlayerPrefs
        {
            get
            {
                var playerPrefsValue = ObscuredPrefs.Get<string>(key: NewsLastShown, defaultValue: String.Empty);
                return long.TryParse(playerPrefsValue, out var ticks) ? new DateTime(ticks: ticks) : DateTime.MinValue;
            }
            set => ObscuredPrefs.Set<string>(key: NewsLastShown, value.Ticks.ToString());
        }
        #endregion

        #region PublicProperties
        public static readonly Dictionary<string, string> CategoryNameDictionary = new()
        {
            { ((int)NewsCategories.Gacha).ToString(), "ガチャ" },
            { ((int)NewsCategories.Event).ToString(), "イベント" },
            { ((int)NewsCategories.Otoku).ToString(), "お得情報" },
            { ((int)NewsCategories.News).ToString(), "お知らせ" },
            { ((int)NewsCategories.Campaign).ToString(), "キャンペーン" },
            { ((int)NewsCategories.Important).ToString(), "重要" },
            { ((int)NewsCategories.Bug).ToString(), "不具合" },
            { ((int)NewsCategories.Maintenance).ToString(), "メンテナンス" },
            { ((int)NewsCategories.Update).ToString(), "アップデート" },
        };

        public static readonly Dictionary<NewsCategories, string> CategoryLogoImageNameDictionary = new()
        {
            { NewsCategories.None, "home_infoIcon_infomation" },
            { NewsCategories.Gacha, "home_infoIcon_gacha" },
            { NewsCategories.Event, "home_infoIcon_event" },
            { NewsCategories.Otoku, "home_infoIcon_sale" },
            { NewsCategories.Campaign, "home_infoIcon_campaign" },
            { NewsCategories.News, "home_infoIcon_infomation" },
            { NewsCategories.Important, "home_infoIcon_important" },
            { NewsCategories.Bug, "home_infoIcon_defect" },
            { NewsCategories.Maintenance, "home_infoIcon_maintenance" },
            { NewsCategories.Update, "home_infoIcon_update" },
        };

        public static readonly HashSet<string> ScheduleNewsCategories = new()
        {
            ((int)NewsCategories.Gacha).ToString(),
            ((int)NewsCategories.Event).ToString(),
            ((int)NewsCategories.Otoku).ToString(),
            ((int)NewsCategories.News).ToString(),
            ((int)NewsCategories.Campaign).ToString(),
        };

        public static readonly HashSet<string> ImportantNewsCategories = new()
        {
            ((int)NewsCategories.Important).ToString(),
            ((int)NewsCategories.Bug).ToString(),
            ((int)NewsCategories.Maintenance).ToString(),
            ((int)NewsCategories.Update).ToString(),
        };
        #endregion

        #region StaticMethods
        public static async void TryShowNews(bool isClickNewsButton, bool isFromTitle, NewsArticleForcedDisplayData newsArticleForcedDisplayData ,Action onComplete, string openArticleUrl = "")
        {
            var newsLastShown = NewsLastShownPlayerPrefs;
            var now = AppTime.Now;
            NewsGetArticleListAPIResponse newsApiResponse = null;
            
            var showDeepLinkNews = false;
            var deepLinkDataContainer = DeepLinkManager.Instance.deepLinkDataContainer;
            if (deepLinkDataContainer is {parameters: { }} &&
                deepLinkDataContainer.parameters.TryGetValue("openNews", out var openNewsValue))
            {
                openArticleUrl = openNewsValue;
                showDeepLinkNews = true;
                DeepLinkManager.Instance.ClearDeepLinkCache();
            }

            // お知らせ強制表示自体があるかの判定
            var showForcedNewsArticle = false;
            // お知らせを強制表示するかどうかの判定
            if (!showDeepLinkNews &&
                newsArticleForcedDisplayData != null &&
                newsArticleForcedDisplayData.NeedToShowNews(newsArticleForcedDisplayData))
            {
                newsApiResponse = await CallNewsApi();
                var forceShowArticleUrl = newsArticleForcedDisplayData.NewsArticleUrl;
                if (newsApiResponse.articleList.Any(aData => aData.url == forceShowArticleUrl))
                {
                    // 最初に表示する記事を設定
                    openArticleUrl = forceShowArticleUrl;
                    showForcedNewsArticle = true;
                    // 保持するデータを更新
                    LocalSaveManager.saveData.NewsArticleForcedDisplayData = newsArticleForcedDisplayData;
                    LocalSaveManager.Instance.SaveData();    
                }
                
            }

            // 日付が変わったら「今日は表示しない」フラグをリセット仕様
            if (newsLastShown.Date < now.Date) NewsDisableTodayAutomaticShowPlayerPrefs = false;
            var newsDisableTodayAutomaticShow = NewsDisableTodayAutomaticShowPlayerPrefs;

            if (isClickNewsButton ||
                showDeepLinkNews ||
                showForcedNewsArticle ||
                ShouldShowNews(lastShownDateTime: newsLastShown, currentDateTime: now,
                    disableTodayAutomaticShow: newsDisableTodayAutomaticShow, isFromTitle: isFromTitle))
            {
                newsApiResponse ??= await CallNewsApi();
                NewsLastShownPlayerPrefs = now;
                
                NewsModalWindow.Open(new NewsModalWindow.WindowParams
                {
                    responseData = newsApiResponse,
                    initialArticleUrl = openArticleUrl,
                    isDisableTodayAutomaticShow = newsDisableTodayAutomaticShow,
                    onChangedNoRepeatToggle = (isDisableAutomaticShow) => NewsDisableTodayAutomaticShowPlayerPrefs = isDisableAutomaticShow,
                    onClosed = onComplete
                });
            }
            else
            {
                onComplete.Invoke();
            }
        }

        public static void TryShowNewsByArticleWithUrl(string url)
        {
            var newsModalInstance = NewsModalWindow.Instance;
            if (newsModalInstance != null)
            {
                newsModalInstance.TryOpenArticle(url);
            }
            else
            {
                TryShowNews(isClickNewsButton: true, onComplete: null, openArticleUrl: url, isFromTitle: false,
                    newsArticleForcedDisplayData: null);
            }
        }

        private static async UniTask<NewsGetArticleListAPIResponse> CallNewsApi()
        {
            var request = new NewsGetArticleListAPIRequest();
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }

        /// <summary>
        /// テスト処理：NewsManagerTest.Logic_ShouldShowNews
        /// </summary>
        public static bool ShouldShowNews(DateTime lastShownDateTime, DateTime currentDateTime, bool disableTodayAutomaticShow, bool isFromTitle)
        {
            return (!disableTodayAutomaticShow && isFromTitle) || lastShownDateTime.Date < currentDateTime.Date; // Dateは年月日まで
        }

        public static List<NewsPoolListItem.ItemParams> CreateNewsPoolListItemParams(NewsGetArticleListAPIResponse response, TabDefinition tabDefinition, Action<NewsPoolListItem.ItemParams> onClickNewsItem)
        {
            var articleList = response.articleList.ToList();
            switch (tabDefinition)
            {
                case TabDefinition.Schedule:
                    articleList = articleList.FindAll(anArticle => ScheduleNewsCategories.Contains(anArticle.category));
                    break;
                case TabDefinition.Important:
                    articleList = articleList.FindAll(anArticle => ImportantNewsCategories.Contains(anArticle.category));
                    break;
            }

            return articleList.Select(anArticle => new NewsPoolListItem.ItemParams(
                onClickItem: onClickNewsItem,
                articleData: anArticle,
                isDetailPage: false
            )).ToList();
        }
        #endregion

        #region InnerClass
        [Serializable]
        public class NewsArticleForcedDisplayData
        {
            public string NewsArticleUrl;
            public string StartAt;
            public string EndAt;

            public NewsArticleForcedDisplayData()
            {
                NewsArticleUrl = string.Empty;
                StartAt = string.Empty;
                EndAt = string.Empty;
            }
            
            public NewsArticleForcedDisplayData(string newsArticleUrl, string startAt, string endAt)
            {
                NewsArticleUrl = newsArticleUrl;
                StartAt = startAt;
                EndAt = endAt;
            }
            
            public bool NeedToShowNews(NewsArticleForcedDisplayData newsArticleForcedDisplayData)
            {
                // お知らせを強制表示するかどうかの判定
                var showForcedNewsArticle = false;
                // 現在のお知らせ強制表示のデータが表示されるかの判定
                // （データがnullではないかつ表示する記事の情報がnullではないかつ表示期間内であるかどうか）
                if (newsArticleForcedDisplayData != null &&
                    !string.IsNullOrEmpty(newsArticleForcedDisplayData.NewsArticleUrl) &&
                    newsArticleForcedDisplayData.StartAt.TryConvertToDateTime().IsPast(AppTime.Now) &&
                    newsArticleForcedDisplayData.EndAt.TryConvertToDateTime().IsFuture(AppTime.Now))
                {
                    var localNewsArticleForcedDisplayData = LocalSaveManager.saveData.NewsArticleForcedDisplayData;
                    // 既に一度でも強制表示をしていた場合保持しているデータと比較して再度表示するかの判定
                    // (保持データがnullまたは今回のデータと保持データの記事情報が違うまたは今回のデータの表示開始時刻と保持したデータの表示開始時刻が同じではない)
                    if (localNewsArticleForcedDisplayData == null || newsArticleForcedDisplayData.NewsArticleUrl !=
                        localNewsArticleForcedDisplayData.NewsArticleUrl || !newsArticleForcedDisplayData.StartAt
                            .TryConvertToDateTime()
                            .IsSame(localNewsArticleForcedDisplayData.StartAt.TryConvertToDateTime()))
                    {
                        showForcedNewsArticle = true;
                    }
                }

                return showForcedNewsArticle;
            }
        }
        #endregion
    }

    #region Enum
    public enum TabDefinition
    {
        LatestNews = 0,
        Schedule = 1,
        Important = 2,
    }

    /// <summary>
    /// <see cref="NewsArticle.category"/>のコメントを参考に 
    /// </summary>
    public enum NewsCategories
    {
        None = 1,
        Gacha = 1001,
        Event = 1002,
        Otoku = 1003,
        News = 1004,
        Campaign = 1005,
        Important = 2001,
        Bug = 2002,
        Maintenance = 2003,
        Update = 2004,
    }
    #endregion
}
