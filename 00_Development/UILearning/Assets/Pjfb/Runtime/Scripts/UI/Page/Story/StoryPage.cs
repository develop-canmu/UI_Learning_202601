using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;

namespace Pjfb.Story
{
    
    public enum StoryPageType
    {
        StoryChapterSelect,
        StoryScenarioSelect,
        StoryDeckSelect,
        Adv
    }

    public class StoryPage : PageManager<StoryPageType>
    {
        #region PageParams
        public class PageParams
        {
            public StoryChapterSelectPage.PageParams nullableChapterPageParam;
            public StoryScenarioSelectPage.PageParams nullableScenarioPageParam;
            public StoryAdvPage.PageParam NullableAdvPageParams;
        }
        #endregion

        #region PrivateProperties
        private PageParams _pageParams;
        #endregion
        
        protected override string GetAddress(StoryPageType page)
        {
            return $"Prefabs/UI/Page/Story/{page}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (TransitionType != PageTransitionType.Back)
            {

                _pageParams = (PageParams) args;
                if (_pageParams.nullableScenarioPageParam != null)
                {
                    await OpenPageAsync(StoryPageType.StoryScenarioSelect, true, _pageParams.nullableScenarioPageParam, token);  
                }
                else if (_pageParams.nullableChapterPageParam != null)
                {
                    await OpenPageAsync(StoryPageType.StoryChapterSelect, true, _pageParams.nullableChapterPageParam, token);
                }
                else if (_pageParams.NullableAdvPageParams != null)
                {
                    await OpenPageAsync(StoryPageType.Adv, true, _pageParams.NullableAdvPageParams, token);
                }
            }
        }

        protected override void OnEnablePage(object args)
        {
            if (_pageParams is {NullableAdvPageParams: { }}) StoryAdvPage.HideHeaderFooter();
            base.OnEnablePage(args);
        }

        #region PublicStatics
        public static void OpenChapterPage(StoryChapterSelectPage.PageParams pageParams)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Story, true, new PageParams{nullableChapterPageParam = pageParams});
        }

        public static void OpenScenarioPage(StoryScenarioSelectPage.PageParams pageParams)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Story, true, new PageParams{nullableScenarioPageParam = pageParams});
        }

        public static void OpenAdvPage(StoryAdvPage.PageParam pageParams)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Story, true, new PageParams{NullableAdvPageParams = pageParams});
        }
        #endregion
    }
}
