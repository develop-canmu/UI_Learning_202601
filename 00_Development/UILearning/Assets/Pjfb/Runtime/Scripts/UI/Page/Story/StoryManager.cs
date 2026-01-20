using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using CruFramework.Page;
using CruFramework.Utils;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Common;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Shop;

namespace Pjfb.Story
{
    public class StoryManager :Singleton<StoryManager>
    {
        #region InnerClass
        private class ResumeData { public HuntUserPending huntUserPending; }
        #endregion
        
        #region PublicProperties
        public ShownStoryHuntEnemyContainer shownStoryHuntEnemyContainer = new ();
        #endregion

        #region PrivateProperties
        private StoryPlayInitiateData nullableStoryPlayInitiateData = null;
        private ResumeData nullableResumeData = null;
        #endregion

        #region PublicMethods
        public void OnClickHomeStoryButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Story, true, null);
        }
        
        public void UpdateResumeData(HuntUserPending huntUserPending)
        {
            nullableResumeData = huntUserPending is {mHuntEnemyId: > 0} ? new ResumeData {huntUserPending = huntUserPending} : null;
        }
        
        public HuntStageMasterObject[] GetCurrentStages(long mHuntId)
        {
            return MasterManager.Instance.huntStageMaster.FindStages(mHuntId);
        }

        public void TryResumeLastStoryBattle(Action onCanceled, Action onSkip)
        {
            CheckResumeDataAvailability(
                onConfirmResume: OnConfirmResume,
                onCancelResume: () => OnCancelResume(onCanceled),
                onSkip: onSkip); // nullableHuntUserPendingデータがnullの場合実行される
        }
        
        public async void OnStoryBattleStart(StoryDeckSelectPage.PageParams deckSelectPageParams)
        {
            var response = await CallHuntStartApi(mHuntEnemyId: deckSelectPageParams.subStoryData.id, deckSelectPageParams.huntTimetableMasterObject.id);
            StartBattle(response.huntPending, deckSelectPageParams);
        }
        #endregion

        #region EventListeners
        public async void OnClickSubStoryScenario(HuntStageMasterObject storyData, long currentProgress, HuntEnemyScenarioMasterObject mScenario, HuntTimetableMasterObject huntTimetableMasterObject, FestivalTimetableMasterObject festivalTimetableMasterObject, FestivalUserStatus festivalUserStatus)
        {
            await OpenAdvPage(mScenario, StoryAdvPage.State.Before, huntTimetableMasterObject.mHuntId);
            var response = await CallHuntStartAndFinishApi(mScenario.mHuntEnemyId, huntTimetableMasterObject.id);
            nullableStoryPlayInitiateData = new StoryPlayInitiateData(currentProgress, storyData, huntTimetableMasterObject, festivalTimetableMasterObject, festivalUserStatus);
            
            var showingRewardWindow = true;
            RewardModal.TryOpen(new RewardModal.Parameters (prizeJsonWraps: response.prizeSetList.SelectMany(aData => aData.prizeJsonList.ToList()).ToList(), onWindowClosed: () => showingRewardWindow = false), ModalOptions.KeepFrontModal);
            while (showingRewardWindow) await UniTask.WaitForFixedUpdate();
            
            JumpPageAfterStoryCleared(progress: response.enemyHistory.progress, huntTimetableMasterObject: huntTimetableMasterObject);
        }
        #endregion
        
        #region ingame coupling methods
        public async UniTask OnStoryBattleFinish(int battleResult, HuntFinishAPIResponse finishResponse)
        {
            var battleResultEnum = (StoryUtility.BattleResultType) battleResult;
            var mHuntEnemyId = finishResponse.mHuntEnemyId;
            
            await TryOpenAfterMatchScenario(battleResultEnum, mHuntEnemyId: mHuntEnemyId, nullableStoryPlayInitiateData.huntTimetableMasterObject.mHuntId);
            
            JumpPageAfterStoryCleared(progress: finishResponse.enemyHistory.progress, nullableStoryPlayInitiateData.huntTimetableMasterObject);
        }

        private async void StartBattle(HuntUserPending huntPending, StoryDeckSelectPage.PageParams deckSelectPageParams)
        {
            await TryOpenBeforeMatchScenario(mHuntEnemyId: deckSelectPageParams.subStoryData.id, deckSelectPageParams.huntTimetableMasterObject.mHuntId);
            OpenInGame(huntPending, deckSelectPageParams);
        }
        
        private void OpenInGame(HuntUserPending huntPending, StoryDeckSelectPage.PageParams deckSelectPageParams)
        {
            // OnStoryBattleFinishに使う
            nullableStoryPlayInitiateData = new StoryPlayInitiateData(storyData: deckSelectPageParams.storyData, currentProgress: deckSelectPageParams.currentProgress, huntTimetableMasterObject: deckSelectPageParams.huntTimetableMasterObject, festivalTimetableMasterObject: deckSelectPageParams.festivalTimetableMasterObject, nullableFestivalUserStatus: deckSelectPageParams.nullableFestivalUserStatus);
            var mHuntTimetable = MasterManager.Instance.huntTimetableMaster.FindData(huntPending.mHuntTimetableId);
            var OpenFrom = mHuntTimetable.type == 1 ? PageType.Rivalry : PageType.Story;

            NewInGameOpenArgs args = null;

            // ライバルリーのみ弱体化編成か調べる
            if (OpenFrom == PageType.Rivalry)
            {
                BigValue fixedHuntEnemyCombatPower = BigValue.Zero;
                
                var huntEnemy = MasterManager.Instance.huntEnemyMaster.FindData(huntPending.mHuntEnemyId);
                if (huntEnemy != null)
                {
                    HuntDeckRegulationType regulationType = RivalryManager.GetRegulationType(huntEnemy.mHuntDeckRegulationId);
                    if (regulationType == HuntDeckRegulationType.Weaken)
                    {
                        // 条件マスタリスト取得
                        IEnumerable<HuntDeckRegulationConditionMasterObject> regulationList = MasterManager.Instance.huntDeckRegulationConditionMaster.values.Where(x => x.mHuntDeckRegulationId == huntEnemy.mHuntDeckRegulationId);

                        // 弱体化条件満たしている
                        if (RivalryManager.CheckHuntCondition(huntPending.clientData, regulationList))
                        {
                            var regulationMaster = MasterManager.Instance.huntDeckRegulationMaster.FindData(huntEnemy.mHuntDeckRegulationId);
                            fixedHuntEnemyCombatPower = new BigValue(regulationMaster.condtionCompleteBonusValue);
                        }
                    }
                }
                
                args = new NewInGameOpenArgs(OpenFrom, huntPending.clientData, fixedHuntEnemyCombatPower, null);
            }
            else
            {
                args = new NewInGameOpenArgs(OpenFrom, huntPending.clientData, null);
            }

            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false, args);
        }

        private async void StartLotteryReward(HuntUserPending huntPending)
        {
            Rivalry.RivalryPage.Data nextParams = new Rivalry.RivalryPage.Data();
            nextParams.pageType = Rivalry.RivalryPageType.RivalryRewardStealChara;
            nextParams.showHeaderAndFooter = false;
            Rivalry.RivalryRewardStealCharaPage.Data postData = new Rivalry.RivalryRewardStealCharaPage.Data();
            postData.huntUserPending = huntPending;
            nextParams.args = postData;
            await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Rivalry, false, nextParams);
        }
        #endregion
        
        #region PrivateMethods
        private void JumpPageAfterStoryCleared(long progress, HuntTimetableMasterObject huntTimetableMasterObject)
        {
            if (nullableStoryPlayInitiateData == null) AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
            else {
                OnFinishSubStory(
                    progress: progress, 
                    storyData: nullableStoryPlayInitiateData.storyData,
                    huntTimetableMasterObject: huntTimetableMasterObject,
                    festivalTimetableMasterObject: nullableStoryPlayInitiateData.festivalTimetableMasterObject,
                    nullableFestivalUserStatus: nullableStoryPlayInitiateData.nullableFestivalUserStatus,
                    progressBefore: nullableStoryPlayInitiateData.currentProgress);
                nullableStoryPlayInitiateData = null;
            }
        }
        
        private void OnFinishSubStory(HuntStageMasterObject storyData, HuntTimetableMasterObject huntTimetableMasterObject, FestivalTimetableMasterObject festivalTimetableMasterObject, FestivalUserStatus nullableFestivalUserStatus, long progress, long progressBefore)
        {
            if (huntTimetableMasterObject.viewEndAt.TryConvertToDateTime().IsPast(AppTime.Now) || nullableFestivalUserStatus == null) {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
                return;
            }
            
            var isFirstTimeClear = progress != progressBefore;
            StoryPage.OpenScenarioPage(new StoryScenarioSelectPage.PageParams(
                storyData: storyData,
                huntTimetableMasterObject: huntTimetableMasterObject,
                festivalTimetableMasterObject: festivalTimetableMasterObject,
                festivalUserStatus: nullableFestivalUserStatus,
                progress: progress,
                showReleaseAnimation: isFirstTimeClear && progress <= storyData.progressMax 
            ));
        }
        
        private void CheckResumeDataAvailability (Action<HuntUserPending> onConfirmResume, Action onCancelResume, Action onSkip)
        {
            if (nullableResumeData == null) {
                onSkip?.Invoke();
                return;
            }

            var huntUserPending = nullableResumeData.huntUserPending;
            nullableResumeData = null;

            AppManager.Instance.UIManager.ModalManager.OpenModal(
                ModalType.PendingConfirm,
                new PendingConfirmModal.Arguments(
                    StringValueAssetLoader.Instance["common.pending_title"],
                    StringValueAssetLoader.Instance["common.pending_body_text"],
                    () => onConfirmResume.Invoke(huntUserPending),
                    onCancelResume,
                    null
                ));
        }

        private void OnConfirmResume(HuntUserPending huntUserPending)
        {
            switch (huntUserPending.state)
            {
                case 2: // 抽選選択画面
                    StartLotteryReward(huntUserPending);
                    break;
                default:
                    var subStoryData = MasterManager.Instance.huntEnemyMaster.FindData(huntUserPending.mHuntEnemyId);
                    var storyData = MasterManager.Instance.huntStageMaster.FindData(subStoryData.mHuntId);
                    var huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindData(huntUserPending.mHuntTimetableId);
                    var festivalTimetableMasterObject = MasterManager.Instance.festivalTimetableMaster.values.FirstOrDefault(aData => aData.mHuntTimetableId == huntTimetableMasterObject.id);
                    StartBattle(huntUserPending, new StoryDeckSelectPage.PageParams{ currentProgress = 0, storyData = storyData, subStoryData = subStoryData, huntTimetableMasterObject = huntTimetableMasterObject, festivalTimetableMasterObject = festivalTimetableMasterObject, nullableFestivalUserStatus = null});
                    break;
            }
        }
        
        private async void OnCancelResume(Action onFinish)
        {
            await CallHuntCancelApi();
            onFinish?.Invoke();
        }

        private async UniTask TryOpenBeforeMatchScenario(long mHuntEnemyId, long mHuntId)
        {
            var mScenario = MasterManager.Instance.huntEnemyScenarioMaster.FindScenario(mHuntEnemyId);
            var beforeScenarioNumber = mScenario?.beforeScenarioNumber ?? 0;
            if (beforeScenarioNumber > 0) await OpenAdvPage(mScenario, StoryAdvPage.State.Before, mHuntId);
        }
        
        private async UniTask TryOpenAfterMatchScenario(StoryUtility.BattleResultType resultType, long mHuntEnemyId, long mHuntId)
        {
            if (resultType != StoryUtility.BattleResultType.Win) return;

            var mScenario = MasterManager.Instance.huntEnemyScenarioMaster.FindScenario(mHuntEnemyId);
            var afterScenarioNumber = mScenario?.afterScenarioNumber ?? 0;
            if (afterScenarioNumber > 0) await OpenAdvPage(mScenario, StoryAdvPage.State.After, mHuntId);
        }

        private async UniTask OpenAdvPage (HuntEnemyScenarioMasterObject mScenario, StoryAdvPage.State state, long mHuntId)
        {
            var showingAdv = true;
            StoryPage.OpenAdvPage(new StoryAdvPage.PageParam {
                mScenario = mScenario,
                showToBeContinued = false,
                state = state,
                onFinish = () => showingAdv = false,
            });
            while (showingAdv) await UniTask.WaitForFixedUpdate();
        }
        #endregion

        #region ApiCallMethods
        public async UniTask<HuntGetTimetableDetailAPIResponse> CallHuntGetTimetableDetailApi(HuntTimetableMasterObject timetableMaster)
        {
            var request = new HuntGetTimetableDetailAPIRequest();
            request.SetPostData(new HuntGetTimetableDetailAPIPost { mHuntTimetableId = timetableMaster.id });
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        /// <summary>
        /// シナリオ一覧画面でシナリオを再生する時のAPI
        /// </summary>
        private async UniTask<HuntStartAndFinishAPIResponse> CallHuntStartAndFinishApi(long mHuntEnemyId, long mHuntTimetableId)
        {
            var requestPendingData = new HuntStartAndFinishAPIRequest();
            requestPendingData.SetPostData(new HuntStartAndFinishAPIPost {
                mHuntEnemyId = mHuntEnemyId,
                mHuntTimetableId = mHuntTimetableId
            });
            await APIManager.Instance.Connect(requestPendingData);
            return requestPendingData.GetResponseData();
        }
        
        /// <summary>
        /// レジュームAPI
        /// </summary>
        private async UniTask<HuntGetDataAPIResponse> CallHuntGetDataApi()
        {
            var requestPendingData = new HuntGetDataAPIRequest();
            requestPendingData.SetPostData(new HuntGetDataAPIPost());
            await APIManager.Instance.Connect(requestPendingData);
            return requestPendingData.GetResponseData();
        }

        /// <summary>
        /// 注意：HuntStartAPIRequestはストーリー以外に、ライバルリーバトルにも利用されます
        /// </summary>
        public static async UniTask<HuntStartAPIResponse> CallHuntStartApi(long mHuntEnemyId, long mHuntTimetableId)
        {
            var request = new HuntStartAPIRequest();
            request.SetPostData(new HuntStartAPIPost {
                mHuntEnemyId = mHuntEnemyId,
                mHuntTimetableId = mHuntTimetableId
            });
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }

        public async UniTask<HuntFinishAPIResponse> CallHuntFinishApi(int battleResult)
        {
            var request = new HuntFinishAPIRequest();
            request.SetPostData(new HuntFinishAPIPost {battleResult = battleResult});
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return request.GetResponseData();
        }

        public async UniTask<HuntCancelAPIResponse> CallHuntCancelApi()
        {
            var request = new HuntCancelAPIRequest();
            request.SetPostData(new HuntCancelAPIPost());
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        #endregion
        
        #region StoryBattleCache
        /// <summary>
        /// ストーリーのダイアログや試合が開始した時に保存されるデータ
        /// </summary>
        private class StoryPlayInitiateData
        {
            public long currentProgress;
            public HuntStageMasterObject storyData;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public FestivalTimetableMasterObject festivalTimetableMasterObject;
            public FestivalUserStatus nullableFestivalUserStatus;
            
            public StoryPlayInitiateData(long currentProgress, HuntStageMasterObject storyData, HuntTimetableMasterObject huntTimetableMasterObject, FestivalTimetableMasterObject festivalTimetableMasterObject, FestivalUserStatus nullableFestivalUserStatus)
            {
                this.currentProgress = currentProgress;
                this.storyData = storyData;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.festivalTimetableMasterObject = festivalTimetableMasterObject;
                this.nullableFestivalUserStatus = nullableFestivalUserStatus;
            }
        }
        #endregion

        #region StoryShownData
        public class ShownStoryHuntEnemyContainer
        {
            private HashSet<long> _mHuntEnemyIds;
            public HashSet<long> mHuntEnemyIds => _mHuntEnemyIds ??= Load();
            
            private const string ShownStoryHuntEnemy = "ShownStoryHuntEnemy";
            private const string CsvSeparator = ",";
            private void Save() => ObscuredPrefs.Set<string>(key: ShownStoryHuntEnemy, mHuntEnemyIds.ToList().ToCsv(separator: CsvSeparator));
            public HashSet<long> Load()
            {
                var prefsValue = ObscuredPrefs.Get<string>(key: ShownStoryHuntEnemy, string.Empty);
                var split = prefsValue.Split(CsvSeparator);
                var selectValue = split.Select(aValue => long.TryParse(aValue, out var mHuntId) ? mHuntId : 0);
                var whereValue = selectValue.Where(mHuntId => mHuntId > 0);
                var retVal = whereValue.ToHashSet();
                return retVal;
            }
            public void Clear() => _mHuntEnemyIds = null;

            public void OnShowingHuntEnemy(long mHuntEnemyId) { if (mHuntEnemyIds.Add(mHuntEnemyId)) Save(); }
        }
        #endregion
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
        #region SRDEBUGで実行されるメソッド
        public async void SRDebug_OnClickForceClear(StoryDeckSelectPage.PageParams deckSelectPageParams)
        {
            // OnStoryBattleFinishに使う
            nullableStoryPlayInitiateData = new StoryPlayInitiateData(deckSelectPageParams.currentProgress, deckSelectPageParams.storyData, deckSelectPageParams.huntTimetableMasterObject, deckSelectPageParams.festivalTimetableMasterObject, deckSelectPageParams.nullableFestivalUserStatus);
            
            var startResponse = await CallHuntStartApi(mHuntEnemyId: deckSelectPageParams.subStoryData.id, mHuntTimetableId: deckSelectPageParams.huntTimetableMasterObject.id);
            
            var showingWindow = true;
            ConfirmModalWindow.Open(new ConfirmModalData("デバッグ実装", $"インゲームを強制クリアします\nmHuntEnemyId:{startResponse.huntPending.mHuntEnemyId}", string.Empty, new ConfirmModalButtonParams("OK", (window) => window.Close(() => showingWindow = false))));
            while (showingWindow) await UniTask.WaitForFixedUpdate();

            var battleResult = (int) StoryUtility.BattleResultType.Win;
            var finishResponse = await CallHuntFinishApi(battleResult: battleResult);
            
            await OnStoryBattleFinish(battleResult, finishResponse);
        }
        #endregion
#endif
    }
}
