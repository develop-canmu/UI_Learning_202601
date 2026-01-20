#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT

using Pjfb.Story;

namespace Pjfb.DebugMenu
{
    public static class DebugMenuStory
    {
        private static readonly string Category = "Story";
        private static StoryDeckSelectPage.PageParams _pageParams;
        
        public static void AddOptions(StoryDeckSelectPage.PageParams pageParams)
        {
            _pageParams = pageParams;
            CruFramework.DebugMenu.AddOption(Category, $"強制クリア mEnemyHuntId:{_pageParams.subStoryData.id}", OnClickForceClear);
        }

        public static void RemoveOptions()
        {
            _pageParams = null;
            CruFramework.DebugMenu.RemoveOption(Category);
        }

        private static void OnClickForceClear()
        {
            StoryManager.Instance.SRDebug_OnClickForceClear(_pageParams);
        }
    }
}

#endif
