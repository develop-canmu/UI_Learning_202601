namespace Pjfb.Story
{
    public abstract class StoryPageBase : Page
    {
        /// <summary>ページ</summary>
        public StoryPage Page{get{return (StoryPage)Manager;}}
    }
}
