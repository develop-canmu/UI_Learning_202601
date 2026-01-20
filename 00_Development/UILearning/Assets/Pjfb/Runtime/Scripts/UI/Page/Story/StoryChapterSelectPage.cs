using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Event;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using Pjfb.UI;
using UnityEngine.UI;

namespace Pjfb.Story
{
    public class StoryChapterSelectPage : StoryPageBase
    {
        #region PageParams
        public class PageParams
        {
            public HuntStageMasterObject[] mStages;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public FestivalTimetableMasterObject festivalTimetableMasterObject;
            public FestivalUserStatus festivalUserStatus;
            public long progress;
            public bool showReleaseAnimation;

            public PageParams(HuntStageMasterObject[] mStages, HuntTimetableMasterObject huntTimetableMasterObject, FestivalTimetableMasterObject festivalTimetableMasterObject, FestivalUserStatus festivalUserStatus, long progress, bool showReleaseAnimation = false)
            {
                this.mStages = mStages;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.festivalTimetableMasterObject = festivalTimetableMasterObject;
                this.festivalUserStatus = festivalUserStatus;
                this.progress = progress;
                this.showReleaseAnimation = showReleaseAnimation;
            }
            
            public void ResetShowReleaseAnimationFlag() => showReleaseAnimation = false;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private PoolListContainer poolListContainer;
        #endregion

        #region PrivateFields
        private PageParams pageParams;
        private CancellationTokenSource source;
        private List<StoryChapterPoolListItem.ItemParams> listItemParams = new();
        private long currentStoryId;
        #endregion

        #region OverrideMethods
        protected override async void OnOpened(object args)
        {
            await ShowPoolList();
            await TryPlayReleaseAnimation();
            base.OnOpened(args);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            pageParams = (PageParams) args;
            poolListContainer.Clear();
            InitListItemParams();
            SetThumbnail();
            await base.OnPreOpen(args, token);
        }

        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            await poolListContainer.SlideOut();
            return await base.OnPreClose(token);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Show(); 
            AppManager.Instance.UIManager.Footer.Show();
            base.OnEnablePage(args);
        }
        
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.OpenStoryTutorialAsync().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        #endregion

        #region PrivateMethods
        private async UniTask ShowPoolList()
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await poolListContainer.SetDataList(listItemParams);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }
        
        private void SetThumbnail()
        {
            if(source != null) {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            thumbnailImage.gameObject.SetActive(false);
            source = new CancellationTokenSource();
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(key: $"Images/StoryThumbnailBanner/story_thumbnail_banner_{currentStoryId}.png",
                callback: sprite => {
                    thumbnailImage.sprite = sprite;
                    thumbnailImage.gameObject.SetActive(true);
                    thumbnailImage.SetNativeSize();
                },
                token: source.Token).Forget();
        }
        
        private async UniTask TryPlayReleaseAnimation()
        {
            if (pageParams.showReleaseAnimation)
            {
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                var animatingItems = poolListContainer.ListItemParams
                    .Where(aData => aData.nullableActiveListItem != null)
                    .Select(aData => (StoryChapterPoolListItem)aData.nullableActiveListItem)
                    .Where(aData => aData.IsNewWithAnimationState);
                foreach (var anItem in animatingItems) await anItem.PlayReleaseAnimation();
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                pageParams.ResetShowReleaseAnimationFlag();
            }
        }

        /// <summary>
        /// 注意：サムネイルに表示するデータを取得するために、InitListItemParams処理はOnPreOpenで実行する
        /// </summary>
        private void InitListItemParams()
        {
            var currentStoryProgress = pageParams.progress;
            listItemParams = pageParams.mStages
                .Where(aData => aData.progressMin <= currentStoryProgress)
                .Select(aData => new StoryChapterPoolListItem.ItemParams(
                    storyData: aData,
                    currentProgress: currentStoryProgress,
                    state: aData.progressMax < currentStoryProgress ? StoryChapterPoolListItem.State.Complete :
                    pageParams.showReleaseAnimation ? StoryChapterPoolListItem.State.NewWithAnimation :
                    StoryChapterPoolListItem.State.New,
                    onClick: OnClickStory))
                .OrderByDescending(aData => aData.storyData.id)
                .ToList();
            currentStoryId = listItemParams.Any() ? listItemParams.Max(aData => aData.storyData.id) : 0;
        }
        #endregion

        #region EventListeners
        public void OnBackButton()
        {
            EventManager.OnClickEventBannerButton(pageParams.festivalTimetableMasterObject.id);
        }

        private void OnClickStory(StoryChapterPoolListItem.ItemParams itemParams)
        {
            if (poolListContainer.isAnimating) return;
            
            var m = (StoryPage)Manager;
            m.OpenPage(StoryPageType.StoryScenarioSelect, true, new StoryScenarioSelectPage.PageParams(storyData: itemParams.storyData, pageParams.huntTimetableMasterObject, pageParams.festivalTimetableMasterObject, festivalUserStatus: pageParams.festivalUserStatus, progress: pageParams.progress));
        }
        
        #endregion
    }
}
