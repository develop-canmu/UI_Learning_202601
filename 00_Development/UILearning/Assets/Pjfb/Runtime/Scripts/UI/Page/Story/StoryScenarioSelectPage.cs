using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Event;
using UnityEngine;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Pjfb.UserData;
using UnityEngine.UI;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;

namespace Pjfb.Story
{
    public class StoryScenarioSelectPage : StoryPageBase
    {
        #region PageParams
        public class PageParams
        {
            public HuntStageMasterObject storyData;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public FestivalTimetableMasterObject festivalTimetableMasterObject;
            public FestivalUserStatus festivalUserStatus;
            public long progress;
            public bool showReleaseAnimation;
            
            public PageParams(HuntStageMasterObject storyData, HuntTimetableMasterObject huntTimetableMasterObject, FestivalTimetableMasterObject festivalTimetableMasterObject, FestivalUserStatus festivalUserStatus, long progress, bool showReleaseAnimation = false)
            {
                this.storyData = storyData;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.festivalTimetableMasterObject = festivalTimetableMasterObject;
                this.festivalUserStatus = festivalUserStatus;
                this.progress = progress;
                this.showReleaseAnimation = showReleaseAnimation;
            }

            public void ResetShowReleaseAnimationFlag() => showReleaseAnimation = false;
        }
        #endregion

        #region SerializeField
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private PoolListContainer poolListContainer;
        #endregion

        #region PrivateFields
        private CancellationTokenSource source = null;
        private PageParams pageParams;
        #endregion

        #region OverrideMethods
        protected override async void OnOpened(object args)
        {
            await ShowPoolList();
            await TryPlayReleaseAnimation();
            base.OnOpened(args);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            pageParams = (PageParams) args;
            poolListContainer.Clear();
            SetThumbnail();
            return base.OnPreOpen(args, token);
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
        #endregion

        #region PrivateMethods
        private void SetThumbnail()
        {
            if(source != null) {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            thumbnailImage.gameObject.SetActive(false);
            source = new CancellationTokenSource();
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(key: $"Images/StoryThumbnailBanner/story_thumbnail_banner_{pageParams.storyData.id}.png",
                callback: sprite => {
                    thumbnailImage.sprite = sprite;
                    thumbnailImage.gameObject.SetActive(true);
                    thumbnailImage.SetNativeSize();
                },
                token: source.Token).Forget();
        }

        private async UniTask ShowPoolList()
        {
            var storyData = pageParams.storyData;
            var subStoryData = MasterManager.Instance.huntEnemyMaster.values;
            var prizesDictionary = MasterManager.Instance.huntEnemyPrizeMaster.values
                .Where(aData => aData.typeEnum == HuntEnemyPrizeMasterObject.Type.FirstTime)
                .GroupBy(aData => aData.mHuntEnemyId)
                .ToDictionary(
                    keySelector: aData => aData.Key,
                    elementSelector: aData => aData.SelectMany(aPrize => aPrize.prizeJson).ToList());

            var currentPoint = pageParams.festivalUserStatus.pointValue;
            var progress = pageParams.progress + 1; // 未解放を一つ表示する
            var listItemParams = subStoryData
                .Where(aData => 
                    aData.mHuntId == storyData.mHuntId && 
                    storyData.progressMax >= aData.progress && aData.progress >= storyData.progressMin &&
                    aData.progress <= progress) // 現在進捗
                .Select(aData => new StoryScenarioPoolListItem.ItemParams(
                    subStoryData: aData,
                    prizeDataList: prizesDictionary.TryGetValue(aData.id, out var prizes) ? prizes : new List<PrizeJsonWrap>(),
                    state:　(aData.keyMPointValue != 0 && aData.keyMPointValue > currentPoint) ? StoryScenarioPoolListItem.State.Lock :
                            aData.progress < pageParams.progress ? StoryScenarioPoolListItem.State.Complete :
                            aData.progress == pageParams.progress && pageParams.showReleaseAnimation ? StoryScenarioPoolListItem.State.NewWithAnimation :
                            aData.progress == pageParams.progress ? StoryScenarioPoolListItem.State.New :
                            StoryScenarioPoolListItem.State.Lock,
                    currentPoint: currentPoint,
                    onClick: OnClickSubStory))
                .ToList();
            listItemParams.Reverse();
            
            // 画面内にロックされるアイテム数は最大１つしかない
            if (listItemParams.Count(anItem => anItem.state == StoryScenarioPoolListItem.State.Lock) > 1)
            {
                listItemParams.RemoveRange(0, count: listItemParams.FindLastIndex(anItem => anItem.state == StoryScenarioPoolListItem.State.Lock));
            }
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await poolListContainer.SetDataList(listItemParams);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }
        
        private async UniTask TryPlayReleaseAnimation()
        {
            if (pageParams.showReleaseAnimation)
            {
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                var animatingItems = poolListContainer.ListItemParams
                    .Where(aData => aData.nullableActiveListItem != null)
                    .Select(aData => (StoryScenarioPoolListItem)aData.nullableActiveListItem)
                    .Where(aData => aData.IsNewWithAnimationState);
                foreach (var anItem in animatingItems) await anItem.PlayReleaseAnimation();
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                pageParams.ResetShowReleaseAnimationFlag();
            }
        }
        #endregion

        #region EventListener
        public void OnBackButton()
        {
            EventManager.OnClickEventBannerButton(pageParams.festivalTimetableMasterObject.id);
        }
        
        
        private void OnClickSubStory(StoryScenarioPoolListItem.ItemParams itemParams)
        {
            if (poolListContainer.isAnimating) return;
            
            StoryManager.Instance.shownStoryHuntEnemyContainer.OnShowingHuntEnemy(itemParams.subStoryData.id);
            
            var subStoryData = itemParams.subStoryData;
            var isBattle = subStoryData.IsBattle;
            var subStoryId = subStoryData.id;
            if(isBattle)
            {
                var m = (StoryPage)Manager;
                m.OpenPage(StoryPageType.StoryDeckSelect, true, new StoryDeckSelectPage.PageParams {
                    currentProgress = pageParams.progress,
                    storyData = pageParams.storyData,
                    subStoryData = subStoryData,
                    huntTimetableMasterObject = pageParams.huntTimetableMasterObject,
                    festivalTimetableMasterObject = pageParams.festivalTimetableMasterObject,
                    nullableFestivalUserStatus = pageParams.festivalUserStatus
                });
                return;
            }
            
            
            var mScenario = MasterManager.Instance.huntEnemyScenarioMaster.FindScenario(subStoryId);
            var beforeScenarioNumber = mScenario?.beforeScenarioNumber ?? 0;
            if (beforeScenarioNumber > 0) StoryManager.Instance.OnClickSubStoryScenario(pageParams.storyData, pageParams.progress, mScenario, pageParams.huntTimetableMasterObject, pageParams.festivalTimetableMasterObject, pageParams.festivalUserStatus);
            else CruFramework.Logger.LogError($"mHuntEnemy:{subStoryData.id} マスタデータが不適切です : バトルなし+シナリオなし");
        }
        #endregion
    }
}
