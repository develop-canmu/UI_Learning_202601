using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Beginner;
using Pjfb.CampaignBanner;
using Pjfb.Colosseum;
using Pjfb.Common;
using Pjfb.Community;
using Pjfb.Event;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.LockedItem;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.News;
using Pjfb.PresentBox;
using Pjfb.RecommendChara;
using Pjfb.Rivalry;
using Pjfb.Shop;
using Pjfb.Storage;
using Pjfb.Story;
using Pjfb.SubscribeInfo;
using Pjfb.SystemUnlock;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine;
using Pjfb.Club;
using Pjfb.InGame;
using Pjfb.InGame.ClubRoyal;
using Pjfb.LeagueMatchTournament;
using Pjfb.Networking.API;
using Pjfb.Ranking;
using Logger = CruFramework.Logger;
using PrizeJsonWrap = Pjfb.Networking.App.Request.PrizeJsonWrap;

namespace Pjfb.Home
{
    public class HomeTopPage : Page
    {
        #region PageParam
        public class PageParam
        {
            /// <summary>
            /// お知らせなどのポップアップが表示するタイミングを判定する
            /// true: OnMessage(EndFade)
            /// false: OnOpened
            /// </summary>
            public bool isOnMessageShowPopups;
            public bool isFromTitle;
            public HomeGetDataAPIResponse homeApiResponse;
        }
        #endregion
        
        #region SerializeFields
        [Header("左上画面")]
        [SerializeField] private UIBadgeNotificationButton missionButton;
        [SerializeField] private UIBadgeNotificationButton communityButton;
        [SerializeField] private UIBadgeNotificationButton recommendCharaButton;
        
        [Header("右上画面")]
        [SerializeField] private UIBadgeNotificationButton newsButton;
        [SerializeField] private UIBadgeNotificationButton presentButton;
        [SerializeField] private HomeLockedItemButton lockedItemButton;
        [SerializeField] private UIBadgeNotificationButton beginnersButton;
        [SerializeField] private HomeSubscribeButton subscribeButton;
        [SerializeField] private SecretSaleButton secretSaleButton;
        
        [Header("左下画面")]
        [SerializeField] private HomeSlideFestivalView slideFestivalView;
        [SerializeField] private HomeSlideCampaignView slideCampaignView;
        [SerializeField] private HomeBeginnerMissionButton beginnerMissionButton;
        [SerializeField] private LeagueMatchBanner leagueMatchBanner;
        [SerializeField] private LeagueMatchTournamentHomeBanner instantTournamentBanner;
        [SerializeField] private UIBadgeNotificationButton rankingButton;
        
        [Header("右下画面")]
        [SerializeField] private UIBadgeBalloonButton trainingButton;
        [SerializeField] private UIBadgeBalloonButton rivalButton;
        [SerializeField] private GameObject colosseumLockObject;
        [SerializeField] private UIBadgeBalloonButton colosseumButton;
        [SerializeField] private AutoTrainingHomeButton autoTrainingButton;

        [Header("真ん中")]
        [SerializeField] private ScrollBanner characterScrollBanner;
        [SerializeField] private HomeCharacterBackground characterBackground;
        #endregion

        #region Instance
        public static HomeTopPage Instance;
        public static void TryUpdateMissionBadge(long finishedMissionCount, MissionCacheData missionCacheData)
        {
            if (Instance != null) Instance.UpdateMissionBadge(finishedMissionCount, missionCacheData);
        }
        #endregion
        
        #region PrivateFields
        private PageParam _pageParam;
        HomeSlideCharaBanner prevChara = null;
        #endregion

        #region OverrideMethods
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        if (_pageParam is {isOnMessageShowPopups: true})
                        {
                            ShowHomePopups();
                        }
                        break;
                }
            }

            return base.OnMessage(value);
        }

        protected override void OnClosed()
        {
            Logger.Log("HomePage.OnClosed");
            base.OnClosed();
        }

        protected override void OnOpened(object args)
        {
            Logger.Log("HomePage.OnOpened");
            base.OnOpened(args);
            if (_pageParam is {isOnMessageShowPopups: false})
            {
                AppManager.Instance.UIManager.Header.Show(); 
                AppManager.Instance.UIManager.Footer.Show();
                ShowHomePopups();
            }
            
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            // Adv再生用のデバッグ
            CruFramework.DebugMenu.AddOption("AdvDebug", "AdvDebug", () =>
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.AdvDebug, false, null);
            });
            
            var i = 0;
            CruFramework.DebugMenu.AddOption<string>(ClubRoyalInGamePageType.BattlePage.ToString(), "デバッグ用接続先URL",
                ()=> PjfbGameHubClient.DebugAddr, (addr) => PjfbGameHubClient.DebugAddr = addr, i++);
            CruFramework.DebugMenu.AddOption<int>(ClubRoyalInGamePageType.BattlePage.ToString(), "デバッグ用接続先Port",
                ()=> PjfbGameHubClient.DebugPort, (port) => PjfbGameHubClient.DebugPort = port, i++);
            CruFramework.DebugMenu.AddOption<int>(ClubRoyalInGamePageType.BattlePage.ToString(), "デバッグ用ギルドID.A",
                ()=> PjfbGameHubClient.DebugGuildIdA, (guildId) => PjfbGameHubClient.DebugGuildIdA = guildId, i++);
            CruFramework.DebugMenu.AddOption<int>(ClubRoyalInGamePageType.BattlePage.ToString(), "デバッグ用ギルドID.B",
                ()=> PjfbGameHubClient.DebugGuildIdB, (guildId) => PjfbGameHubClient.DebugGuildIdB = guildId, i++);
            CruFramework.DebugMenu.AddOption<int>(ClubRoyalInGamePageType.BattlePage.ToString(), "デバッグデッキID",
                ()=> PjfbGameHubClient.DebugDeckId, (deckId) => PjfbGameHubClient.DebugDeckId = deckId, i++);
            CruFramework.DebugMenu.AddOption(ClubRoyalInGamePageType.BattlePage.ToString(), "ギルバト遷移", () =>
            {
                var ingameOpenArgs = new ClubRoyalInGameOpenArgs(PjfbGameHubClient.DebugAddr, PjfbGameHubClient.DebugPort,
                    PjfbGameHubClient.DebugGuildIdA, PjfbGameHubClient.DebugGuildIdB, PjfbGameHubClient.DebugDeckId);
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyalInGame, false, ingameOpenArgs);
            }, i++);
            
            CruFramework.DebugMenu.AddOption("シークレットショップ", "シークレットポップアップモーダル表示", () =>
            {
                SecretBannerModalWindow.OpenModalTest().Forget();
            });

            // コンテンツリザルト再生用のデバッグ
            CruFramework.DebugMenu.AddOption("コンテンツリザルト", "リザルト演出確認", () =>
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.HomeDebug, false, new HomeDebugPage.Arguments(HomeDebugPageType.Result));
            });
            
#endif
            
        }

        protected override void OnEnablePage(object args)
        {
            Logger.Log("HomePage.OnEnablePage");
            base.OnEnablePage(args);
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            Logger.Log("HomePage.OnPreClose");
            return base.OnPreClose(token);
        }

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Logger.Log("HomePage.OnPreOpen");
            await Init(pageParam: (PageParam) args);
            await base.OnPreOpen(args, token);
        }

        private void Awake()
        {
            characterScrollBanner.onChangedPage += OnCharacterScrollBannerChangedPage;
            Instance = this;
        }

        protected override void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }
        #endregion

        #region PrivateMethods
        private async UniTask Init(PageParam pageParam)
        {
            _pageParam = pageParam;
            var homeApiResponse = _pageParam.homeApiResponse;
            // 解放演出の一時的な既読フラグを初期化する
            SystemUnlockDataManager.Instance.ResetAllTempData();
            UpdateMissionBadge(homeApiResponse.finishedMissionCount);
            UpdateCommunityBadge();
            UpdateRecommendCharaBadge();
            // ランキング開催中か判断してボタン表示を切り替える
            bool holdingRankingFlg = RankingManager.IsHoldingRanking;
            if (holdingRankingFlg)
            {
                rankingButton.gameObject.SetActive(true);
                UpdateRankingBadge();
            }
            else
            {
                rankingButton.gameObject.SetActive(false);
            }
            newsButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: false, onClick: OnClickNews));
            presentButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: homeApiResponse.unreceivedGiftCount > 0, onClick: OnClickPresent));
            
            UpdateLockedItemButton(homeApiResponse.unreceivedGiftLockedCount, homeApiResponse.unopenedGiftBoxCount, homeApiResponse.newestGiftLockedAt);
            beginnersButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: BeginnerManager.showHomeBadge, onClick: OnClickBeginnersButton));
            beginnersButton.gameObject.SetActive(false); // TODO: 初心者は一旦非表示

            var tagObj = UserDataManager.Instance.tagObj;
            var loginPass = MasterManager.Instance.loginPassMaster.values.ToList();
            subscribeButton.Init(new HomeSubscribeButton.Parameters(uTagObj: tagObj, mLoginPass: loginPass, onClickButton: OnClickSubscribeInfoButton));
            slideFestivalView.Init(MasterManager.Instance.festivalTimetableMaster, OnClickSlideFestivalBanner).Forget();
            slideCampaignView.SetBannerDatas(homeApiResponse.newsBannerList);

            UpdateBeginnerMissionButton(homeApiResponse.beginnerMissionCategoryStatus);
            trainingButton.Init(new UIBadgeBalloonButton.ButtonParams(badgeString: TrainingManager.homeBadgeLabel, showBadgeNotification: false, onClick: OnClickTrainingButton));
            rivalButton.Init(new UIBadgeBalloonButton.ButtonParams(badgeString: RivalryManager.GetHomeRivalryButtonNotificationLabel(), showBadgeNotification: RivalryManager.GetHomeBadgeFlag(), onClick: OnClickRivalButton));
            //leagueMatchButton.Init(new UIBadgeBalloonButton.ButtonParams(badgeString: "null", showBadgeNotification: LeagueMatchManager.GetHomeBadgeFlag(), onClick: OnClickLeagueMatchButton));
            autoTrainingButton.Init(homeApiResponse.trainingAutoPendingStatusList);
            
            var unlockColosseum = ColosseumManager.IsUnLockColosseum();
            colosseumLockObject.gameObject.SetActive(!unlockColosseum);
            colosseumButton.Init(new UIBadgeBalloonButton.ButtonParams(badgeString: ColosseumManager.GetHomeColosseumButtonNotificationLabel(), showBadgeNotification: ColosseumManager.GetHomeBadgeFlag(), onClick: OnClickColosseumButton));
            
            UpdateLeagueMatchBanner();
            
            await SetCharacterScrollBanner();
            PrepareUnlockSystem(homeApiResponse);
            
            // シークレットセールボタンの表示
            NativeApiSaleIntroduction[] saleData = ShopManager.GetSaleIntroductionsOrderPriority();
            if (saleData.Length > 0)
            {
                secretSaleButton.gameObject.SetActive(true);
                secretSaleButton.SetSecretButton();
            }
            else
            {
                secretSaleButton.gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// 汎用解放システムを準備します。
        /// 他画面からの別導線モーダル出力を回避するために、事前にフラグを立てます。
        /// </summary>
        /// <param name="data">API data</param>
        private void PrepareUnlockSystem(HomeGetDataAPIResponse data)
        {
            if (data.unViewedSystemLockNumberList.Length > 0)
            {
                SystemUnlockDataManager.Instance.EffectPrepare();
            }
        }

        private void CheckUnlockSystem(HomeGetDataAPIResponse data)
        {
            var unViewedSystemLockNumberList = data.unViewedSystemLockNumberList;
            if (unViewedSystemLockNumberList.Length == 0)
            {
                return;
            }

            // priority順かつ、未再生エフェクトは除外されているためソートしない
            var unlockData = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(unViewedSystemLockNumberList.First());
            
            var param = new SystemUnlockView.Param()
            {
                systemNumber = unlockData.systemNumber,
                systemLockMasterObject = unlockData,
            };
            
            SystemUnlockDataManager.Instance.Unlock(param).Forget();
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            // 汎用解放演出を中止して待機中の機能を一括解放するメニューを追加
            string unlockCategory = "SystemUnlock";
            string unlockName = "システム一括解放";
            CruFramework.DebugMenu.AddOption(unlockCategory, unlockName, async () =>
            {
                foreach (long systemNumber in unViewedSystemLockNumberList)
                {
                    // 番号を更新
                    SystemUnlockDataManager.Instance.UpdateTempUnlockSystemNumber(systemNumber);
                    // 既読API
                    await SystemUnlockDataManager.Instance.RequestReadUnlockEffectAsync();
                }
                
                // メニューを削除
                CruFramework.DebugMenu.RemoveOption(unlockCategory, unlockName);
                
                // ボタンを戻す
                AppManager.Instance.UIManager.EffectManager.SystemUnlockView.RestoreButtonWhenCancel();
                
                // 演出を止める
                AppManager.Instance.UIManager.EffectManager.SystemUnlockView.EndEffectAndDestroy();
                
                // データを更新するために親ページから再読み込み
                HomePage.PageParameters pageParameters = new HomePage.PageParameters()
                {
                    isFromTitle = false,
                };
                AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, false, pageParameters).Forget();
            });
#endif
        }

        private async UniTask SetCharacterScrollBanner()
        {
            // メモ：ここではAPIを通信することはない想定、user/getDataでキャッシュ作成された
            var deckList = await DeckUtility.GetDeckList(DeckType.Battle);
            var selectedDeckPartyNumber = deckList.DeckDataList[0].PartyNumber; // メモ：最初のデッキを表示する仕様になってます
            var selectedDeckData = deckList.DeckDataList.ToList().Find(aDeck => aDeck.Deck.partyNumber == selectedDeckPartyNumber);
            if (selectedDeckData == null) characterScrollBanner.gameObject.SetActive(false);
            else {
                var bannerDataList = selectedDeckData.Deck.memberIdList
                    .Select(aChara => new HomeSlideCharaBanner.Parameters(charaMasterObject: UserDataManager.Instance.charaVariable.Find(aChara.l[1]).MChara))
                    .ToList();
                characterScrollBanner.SetBannerDatas(bannerDataList);
                characterScrollBanner.gameObject.SetActive(true);
                
                // 最初に表示されるキャラのエフェクトを再生する
                var currentParamKey = (HomeSlideCharaBanner.Parameters)characterScrollBanner.GetNullableBannerData();
                SetEffectStatus(currentParamKey);
                
                await SetCurrentCharacterCardBackgroundImage();
            }
        }
        
        private async UniTask SetCurrentCharacterCardBackgroundImage()
        {
            var nullableBannerData = characterScrollBanner.GetNullableBannerData();
            if (nullableBannerData == null) characterBackground.gameObject.SetActive(false);
            else {
                var bannerData = (HomeSlideCharaBanner.Parameters) nullableBannerData;
                await characterBackground.SetDisplay(new HomeCharacterBackground.Parameters{charaMasterObject = bannerData.charaMasterObject});
                characterBackground.gameObject.SetActive(true);
            }
        }

        private void OnReceiveMissionReward(long finishedMissionCount, long unreceivedGiftLockedCount, long unopenedGiftBoxCount, string newestGiftLockedAt, MissionCacheData missionCacheData)
        {
            LockedItemManager.unreceivedGiftCount = unreceivedGiftLockedCount;
            UpdateLockedItemButton(unreceivedGiftLockedCount, unopenedGiftBoxCount, newestGiftLockedAt);
            UpdateMissionBadge(finishedMissionCount, missionCacheData);
        }
        
        private void UpdateMissionBadge(long finishedMissionCount, MissionCacheData missionCacheData = null)
        {
            var showHomeBadge = finishedMissionCount > 0;
            missionButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: showHomeBadge, onClick: OnClickMissionButton));

            MissionManager.finishedMissionCount = finishedMissionCount;
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
        
        protected virtual void ShowHomePopups()
        {
            TryResumeLastStoryBattle(
            nextAction: () => TryShowNews(isFromTitle: _pageParam.isFromTitle,
            nextAction: () => TryShowCampaignBanners(
            nextAction: () => TryColosseumResult(
            nextAction: () => TryClubMatchResult(
            nextAction: () => TryLeagueMatchResult(
            nextAction: () => TryClubRoyalSeasonResult(    
            nextAction: () => TryInstantTournamentResult(
            nextAction: () => TryShowSaleIntroduction(
            nextAction: () => TryShowLockedAnimation (
            nextAction: () => TryShowBeginnerMissionModal (
            nextAction: () => TryUnlockSystem(
            nextAction: null ))))))))))));
        }

        private void TryResumeLastStoryBattle(Action nextAction) => StoryManager.Instance.TryResumeLastStoryBattle(onCanceled: nextAction, onSkip: nextAction);

        public void TryShowNews(bool isFromTitle, Action nextAction) => NewsManager.TryShowNews(
            isClickNewsButton: false, onComplete: nextAction, isFromTitle: isFromTitle,
            newsArticleForcedDisplayData: new NewsManager.NewsArticleForcedDisplayData(
                _pageParam.homeApiResponse.newsArticleForcedDisplayTarget,
                _pageParam.homeApiResponse.newsArticleForcedDisplayStartAt,
                _pageParam.homeApiResponse.newsArticleForcedDisplayEndAt));
        private void TryShowCampaignBanners(Action nextAction) => CampaignBannerManager.TryShowCampaignBanner(newsPopupList: _pageParam.homeApiResponse.newsPopupList, shownData: CampaignBannerManager.ShownPopupDataContainer.LoadFromPlayerPrefs(), onComplete: nextAction);
        private void TryColosseumResult(Action nextAction) => ColosseumManager.TrySeasonResult(handlingType: ColosseumClientHandlingType.PvP ,onFinish: nextAction);
        private void TryClubMatchResult(Action nextAction) => ColosseumManager.TrySeasonResult(handlingType: ColosseumClientHandlingType.ClubMatch ,onFinish: nextAction);
        private void TryLeagueMatchResult(Action nextAction) => ColosseumManager.TrySeasonResult(handlingType: ColosseumClientHandlingType.LeagueMatch, onFinish: nextAction);
        private void TryClubRoyalSeasonResult(Action nextAction) => ColosseumManager.TrySeasonResult(handlingType: ColosseumClientHandlingType.ClubRoyal, onFinish: nextAction);
        private void TryInstantTournamentResult(Action nextAction) => ColosseumManager.TrySeasonResult(handlingType: ColosseumClientHandlingType.InstantTournament, nextAction);
        private void TryShowLockedAnimation(Action nextAction) => lockedItemButton.TryShowFlashAnimation(nextAction).Forget();
        private void TryShowSaleIntroduction(Action nextAction) => ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.HomeTop, onFinish: nextAction);
        private void TryShowBeginnerMissionModal(Action nextAction) => MissionManager.Instance.TryShowBeginnerMissionModal(_pageParam.homeApiResponse.beginnerMissionCategoryStatus, nextAction);
        private void TryUnlockSystem(Action nextAction) => CheckUnlockSystem(_pageParam.homeApiResponse);
        #endregion
        
        #region EventListeners
        private void OnClickMissionButton()
        {
            Logger.Log("HomePage.OnClickMissionButton");
            MissionManager.Instance.OnClickMissionButton(
                onReceiveMissionReward: OnReceiveMissionReward,
                onWindowClosed: () => lockedItemButton.TryShowFlashAnimation().Forget());
        }

        private void OnClickCommunityButton()
        {
            Logger.Log("HomePage.OnClickCommunityButton");
            CommunityManager.OnClickCommunityButton();
        }

        private void OnClickRecommendCharaButton()
        {
            Logger.Log("HomePage.OnClickRecommendCharaButton");
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.RecommendChara, true, null);
        }
        
        private void OnClickRankingButton()
        {
            Logger.Log("HomePage.OnClickRankingButton");
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Ranking, true, null);
        }

        private void OnClickNews()
        {
            Logger.Log("HomePage.OnClickNews");
            NewsManager.TryShowNews(isClickNewsButton: true, onComplete: null, isFromTitle: false,
                newsArticleForcedDisplayData: null);
        }
        
        private void OnClickPresent()
        {
            Logger.Log("HomePage.OnClickPresent");
            PresentBoxManager.OpenPresentBoxModalWindow(onReceivePresentComplete: OnReceivePresentComplete, initialScrollValue: 1f);
        }

        private async void OnReceivePresentComplete(long unreceivedGiftCount, long unreceivedGiftLockedCount, long unopenedGiftBoxCount, string newestGiftLockedAt, PrizeJsonWrap[] prizeJsonList, float lastScrollValue)
        {
            presentButton.ShowBadge(unreceivedGiftCount > 0);

            PresentBoxManager.unreceivedGiftCount = unreceivedGiftCount;
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
            
            LockedItemManager.unreceivedGiftCount = unreceivedGiftLockedCount;
            UpdateLockedItemButton(unreceivedGiftLockedCount, unopenedGiftBoxCount, newestGiftLockedAt);
            await lockedItemButton.TryShowFlashAnimation();
            
            // 受け取れるものがない場合はプレゼントボックスを開かない
            if(unreceivedGiftCount <= 0) return;
            
            PresentBoxManager.OpenPresentBoxModalWindow(onReceivePresentComplete: OnReceivePresentComplete, initialScrollValue: lastScrollValue);
        }

        private void OnClickLockedItemButton(HomeLockedItemButton.Parameters parameters)
        {
            Logger.Log("HomePage.OnClickLockedItemButton");
            LockedItemManager.OpenLockedItemModalWindow(
                onUpdatedLockParameters: UpdateLockedItemButton,
                onReceivePresentComplete: OnReceiveLockedItemComplete,
                initialScrollValue: 1f);
        }

        private void UpdateBeginnerMissionButton(MissionCategoryStatus beginnerMissionCategoryStatus)
        {
            beginnerMissionButton.Init(new HomeBeginnerMissionButton.Parameters(beginnerMissionCategoryStatus, onClick: OnClickBeginnerButton));
        }
        
        private void UpdateLockedItemButton(long unreceivedGiftCount, long unopenedGiftBoxCount, string newestGiftLockedAt)
        {
            lockedItemButton.Init(new HomeLockedItemButton.Parameters(unreceivedGiftCount, unopenedGiftBoxCount, newestGiftLockedAt, OnClickLockedItemButton));
            
            LockedItemManager.unreceivedGiftCount = unreceivedGiftCount;
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
        
        private void OnReceiveLockedItemComplete(float lastScrollValue)
        {
            LockedItemManager.OpenLockedItemModalWindow(
                onUpdatedLockParameters: UpdateLockedItemButton, 
                onReceivePresentComplete: OnReceiveLockedItemComplete,
                initialScrollValue: lastScrollValue);
        }
        
        private void OnClickBeginnersButton()
        {
            Logger.Log("HomePage.OnClickBeginnersButton");
            beginnersButton.ShowBadge(false);
            BeginnerManager.OnClickBeginnerButton();
        }
        
        private void OnClickSubscribeInfoButton(HomeSubscribeButton.Parameters parameters)
        {
            Logger.Log("HomePage.OnClickSubscribeInfoButton");
            SubscribeInfoModal.Open(new SubscribeInfoModal.Parameters(parameters.orderedOpenTagObject, onWindowClosed:null));
        }
        
        private void OnClickStoryButton()
        {
            Logger.Log("HomePage.OnClickStoryButton");
            StoryManager.Instance.OnClickHomeStoryButton();
        }

        private void OnClickBeginnerButton(HomeBeginnerMissionButton.Parameters parameters)
        {
            Logger.Log("HomePage.OnClickBeginnerButton");
            if (parameters.beginnerMissionCategoryStatus.targetMission.progressStatus == (int) MissionProgressStatus.ReceivingReward) {
                MissionManager.Instance.ReceiveBeginnerMission(parameters, onFinish: UpdateBeginnerMissionButton);
            } else {
                MissionManager.Instance.ShowBeginnerMissionModal(parameters);
            }
        }
        
        private void OnClickTrainingButton()
        {
            Logger.Log("HomePage.OnClickTrainingButton");
            trainingButton.ShowBadge(false);
            TrainingManager.OnClickTrainingButton();
        }

        private void OnClickRivalButton()
        {
            Logger.Log("HomePage.OnClickRivalButton");
            // サポート器具上限チェック
            if (SupportEquipmentManager.ShowOverLimitModal()) 
            {
                return;
            }
            RivalryManager.OnClickRivalryButton();
        }
        
        private void OnClickColosseumButton()
        {
            Logger.Log("HomePage.OnClickPvpButton");
            colosseumButton.ShowBadge(false);
            ColosseumPage.OpenPage(true);
        }
        
        /// <summary>リーグマッチのバナーセット</summary>
        void UpdateLeagueMatchBanner()
        {
            // リーグマッチ
            LeagueMatchInfo data = LeagueMatchUtility.GetLeagueMatchInfo(ColosseumClientHandlingType.LeagueMatch);
            leagueMatchBanner.SetView(data);
            
            // 大会バナー
            List<LeagueMatchTournamentInfo> tournamentInfoList = LeagueMatchTournamentManager.GetTournamentList();
            instantTournamentBanner.SetView(tournamentInfoList);
        }
        
        private void OnClickSlideFestivalBanner(HomeSlideFestivalBanner.Parameters parameters)
        {
            var now = AppTime.Now;
            if (parameters.mFestivalTimetable.endAt.TryConvertToDateTime().IsFuture(now) && 
                parameters.mHuntTimetable.endAt.TryConvertToDateTime().IsFuture(now))
            {
                Logger.Log($"HomeTopPage.OnClickSlideFestivalBanner mFestivalTimetableId:{parameters.mFestivalTimetable.id} mFestivalId:{parameters.mFestival.id}");
                EventManager.OnClickEventBannerButton(parameters.mFestivalTimetable.id);
            }
            else
            {
                ConfirmModalWindow.Open(
                    data: new ConfirmModalData(
                        title: StringValueAssetLoader.Instance["event.home.term_ended_title"],
                        message: StringValueAssetLoader.Instance["event.home.term_ended_message"], 
                        caution: string.Empty,
                        buttonParams: new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], onClick: modal => modal.Close())));
            }
        }
        
        // 外部決済サイトへの誘導ボタン
        public void OnClickExternalPurchaseButton()
        {
            ShopManager.OpenExternalPurchaseSite();
        }

        private void OnCharacterScrollBannerChangedPage(int index)
        {
            SetCurrentCharacterCardBackgroundImage().Forget();

            // 表示されているキャラの情報を取得
            var currentParam = (HomeSlideCharaBanner.Parameters)characterScrollBanner.GetNullableBannerData();
            SetEffectStatus(currentParam);
        }
        
        /// <summary>
        /// エフェクトつきキャラが表示された時に再生する
        /// </summary>
        /// <param name="currentParam">表示キャラの情報</param>
        private void SetEffectStatus(HomeSlideCharaBanner.Parameters currentParam)
        {
            // 前のキャラ情報を保持している場合は、エフェクトを停止する
            if (prevChara != null)
            {
                prevChara.CharacterCardHomeImage.StopEffect(currentParam.charaMasterObject.id);
            }
            
            // 表示オブジェクト取得
            var nextChara = (HomeSlideCharaBanner)characterScrollBanner.ScrollGrid.GetItem(currentParam);
            // 画像orエフェクト読み込み
            nextChara.CharacterCardHomeImage.SetTextureAsync(currentParam.charaMasterObject.id).Forget();
            
            // 前キャラ更新
            prevChara = nextChara;
        }

        public void UpdateCommunityBadge()
        {
            communityButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: CommunityManager.showHomeBadge, onClick: OnClickCommunityButton));
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }

        public void UpdateRecommendCharaBadge()
        {
            recommendCharaButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: RecommendCharaManager.showHomeBadge, onClick: OnClickRecommendCharaButton));
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
        
        public void UpdateRankingBadge()
        {
            rankingButton.Init(new UIBadgeNotificationButton.ButtonParams(showBadge: RankingManager.IsShowHomeBadge, onClick: OnClickRankingButton));
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }

        #endregion

#if UNITY_EDITOR || UNITY_ANDROID
        #region BackKey処理
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && 
                AppManager.Instance.UIManager.ModalManager.GetTopModalWindow() == null && 
                !AppManager.Instance.UIManager.System.TouchGuard.isActiveAndEnabled &&
                !AppManager.Instance.UIManager.System.Loading.isActiveAndEnabled)
            {
                SEManager.PlaySE(SE.se_common_tap);
                MenuModalWindow.OnClickBackTitleButton();
            }
            
        }
        #endregion
#endif // UNITY_EDITOR || UNITY_ANDROID
    }
}
