using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.LockedItem;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Story;
using Pjfb.Training;
using Pjfb.UserData;
using TMPro;
using UniRx;
using UnityEngine.UI;
using Pjfb.EventRanking;
using Pjfb.Storage;
using Logger = CruFramework.Logger;
using PrizeJsonWrap = Pjfb.Networking.App.Request.PrizeJsonWrap;

namespace Pjfb.Event
{
    public class EventTopPage : Page
    {
        #region Params

        public class Data
        {
            public readonly long FestivalTimetableId;

            public Data(long festivalTimetableId)
            {
                FestivalTimetableId = festivalTimetableId;
            }
        }

        #endregion
        
        [SerializeField] private EventTopBackgroundImage eventTopBackgroundImage;
        [SerializeField] private EventLogoImage eventLogoImage;
        [SerializeField] private EventTopCharacterImage eventTopCharacterImage;
        [SerializeField] private TMP_Text periodText;
        [SerializeField] private TMP_Text possessionPointText;
        [SerializeField] private OmissionTextSetter possessionPointOmissionTextSetter;
        [SerializeField] private GameObject eventBonusRemainTimeRoot;
        [SerializeField] private TMP_Text eventBonusRemainTimeText;
        [SerializeField] private UIButton eventBonusButton;
        [SerializeField] private UIButton eventBoostButton;
        [SerializeField] private EventTopStoryButtonImage eventTopStoryButtonImageImage;
        [SerializeField] private GameObject storyButtonBadge;
        [SerializeField] private GameObject nextStoryTextRoot;
        [SerializeField] private TMP_Text nextStoryPointText;
        [SerializeField] private GameObject allReleaseStoryText;
        [SerializeField] private TMP_Text storyReleaseText;
        [SerializeField] private EventTopEventPointRewardButtonImage eventTopEventPointRewardButtonImage;
        [SerializeField] private GameObject eventPointRewardButtonBadge;
        [SerializeField] private GameObject nextText;
        [SerializeField] private GameObject arrowText;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject nextPointRewardTextRoot;
        [SerializeField] private TMP_Text nextEventPointText;
        [SerializeField] private GameObject allReceivePointRewardText;
        [SerializeField] private GameObject eventMissionButtonBadge;
        [SerializeField] private UIButton eventMissionButton;
        [SerializeField] private UIButton trainingButton;
        [SerializeField] private UIButton rankingFilter;
        [SerializeField] private GameObject gachaFilter;
        
        private Data pageData;
        private FestivalGetFestivalDetailAPIResponse getFestivalDetailAPIResponse;
        private FestivalUserStatus FestivalUserStatus => getFestivalDetailAPIResponse?.userStatus;
        private List<FestivalSpecificCharaMasterObject> boostCharaList;
        private long minTrainingScenarioId;
        private IDisposable updateExitTimeDisposable;
        private bool readdExitTimeEvent;
        private long GetEventMissionCategoryId {
            get
            {
                var festivalTimetableMasterObject = MasterManager.Instance.festivalTimetableMaster.FindData(getFestivalDetailAPIResponse.userStatus.mFestivalTimetableId);
                return festivalTimetableMasterObject is {mDailyMissionCategoryIdList: {Length: >= 2}} ? festivalTimetableMasterObject.mDailyMissionCategoryIdList[1] : 0;
            }
        }

        private FestivalTimetableMasterObjectBase mFestivalTimeTableCache;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            pageData = (Data)args;
            await GetFestivalDetailAPI(pageData.FestivalTimetableId);
            Init();
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            if (readdExitTimeEvent) AddExitTimeEvent();
            // 初回遷移の場合イベントヘルプを表示する
            if (!HasNewConfirmedFestival()) return;
            OnClickEventHelpButton();
            SaveConfirmedFestivalId();
        }

        protected override void OnClosed()
        {
            DisposeExitTimeEvent(); 
            readdExitTimeEvent = true;  
        }

        private void Init()
        {
            if (getFestivalDetailAPIResponse == null || 
                FestivalUserStatus == null ||
                FestivalUserStatus.huntTimetableStatus == null ||
                FestivalUserStatus.prizeMissionStatus == null)
            {
                CruFramework.Logger.LogError("getFestivalDetailAPIResponse関連のデータがnullです。　クライアントに確認をお願いします");
                return;
            }
            
            mFestivalTimeTableCache = MasterManager.Instance.festivalTimetableMaster.FindData(FestivalUserStatus.mFestivalTimetableId);
            if (mFestivalTimeTableCache == null) return;
            eventTopBackgroundImage.SetTexture(FestivalUserStatus.mFestivalId);
            eventLogoImage.SetTexture(FestivalUserStatus.mFestivalId);
            eventTopCharacterImage.SetTexture(FestivalUserStatus.mFestivalId);
            var sb = new StringBuilder();
            periodText.text = sb.AppendFormat(StringValueAssetLoader.Instance["event.period"],
                mFestivalTimeTableCache.startAt.TryConvertToDateTime().GetNewsDateTimeString(),
                mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().GetNewsDateTimeString()).ToString();
            possessionPointText.text = new BigValue(FestivalUserStatus.pointValue).ToDisplayString(possessionPointOmissionTextSetter.GetOmissionData());
            eventBonusRemainTimeRoot.SetActive(HasRemainBoostBonusTime());
            var isDeadlineExceeded = mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            eventBonusButton.interactable = !isDeadlineExceeded && !HasRemainTime();
            AddExitTimeEvent();
            // ブーストキャラ
            minTrainingScenarioId = MasterManager.Instance.festivalSpecificCharaMaster.values.Aggregate((min,next) => min.mTrainingScenarioId < next.mTrainingScenarioId ? min : next).mTrainingScenarioId;
            boostCharaList = MasterManager.Instance.festivalSpecificCharaMaster.values.Where(x => x.mFestivalId == FestivalUserStatus.mFestivalId && x.mTrainingScenarioId == minTrainingScenarioId).ToList();
            
            eventBoostButton.interactable = !isDeadlineExceeded && boostCharaList != null && boostCharaList.Count > 0;
            eventTopStoryButtonImageImage.SetTexture(FestivalUserStatus.mFestivalId);
            storyButtonBadge.SetActive(FestivalUserStatus.huntTimetableStatus.hasNotification);
            var isAllReleaseStory = FestivalUserStatus.huntTimetableStatus.openEnemyCount == FestivalUserStatus.huntTimetableStatus.enemyCount;
            nextStoryTextRoot.SetActive(!isAllReleaseStory);
            nextStoryPointText.text = FestivalUserStatus.huntTimetableStatus.nextPointValue.ToString();
            allReleaseStoryText.SetActive(isAllReleaseStory);
            sb = new StringBuilder();
            storyReleaseText.text = sb.AppendFormat(StringValueAssetLoader.Instance["common.ratio_value"],
                FestivalUserStatus.huntTimetableStatus.openEnemyCount,
                FestivalUserStatus.huntTimetableStatus.enemyCount).ToString();
            eventTopEventPointRewardButtonImage.SetTexture(FestivalUserStatus.mFestivalId);
            eventPointRewardButtonBadge.SetActive(FestivalUserStatus.prizeMissionStatus.hasNotification);
            nextEventPointText.text = FestivalUserStatus.prizeMissionStatus.nextPointValue.ToString();
            // 先頭の報酬を表示する
            var prizeJson = FestivalUserStatus.prizeMissionStatus.nextPrizeList.FirstOrDefault();
            var hasPrize = prizeJson != null;
            nextText.SetActive(hasPrize);
            arrowText.SetActive(hasPrize);
            prizeJsonView.gameObject.SetActive(hasPrize);
            if (hasPrize)
            {
                prizeJsonView.SetView(prizeJson);
            }
            nextPointRewardTextRoot.SetActive(hasPrize);
            allReceivePointRewardText.SetActive(!hasPrize);
            eventMissionButtonBadge.SetActive(FestivalUserStatus.hasMissionNotification);
            eventMissionButton.interactable = GetEventMissionCategoryId != 0;
            trainingButton.interactable = !isDeadlineExceeded;

            rankingFilter.gameObject.SetActive(!IsOpenRanking());
            gachaFilter.gameObject.SetActive(!IsOpenGacha());

            confirmedFestivalIdList = null;
        }

        private void AddExitTimeEvent()
        {
            var isDeadlineExceeded =　mFestivalTimeTableCache == null || mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            if (isDeadlineExceeded || !HasRemainTime()) return;
            DisposeExitTimeEvent();
            SetExitTimeText();
            updateExitTimeDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                SetExitTimeText();
            }).AddTo(gameObject);
            readdExitTimeEvent = false;
        }
        
        private void SetExitTimeText()
        {
            var endDate = FestivalUserStatus.effectStatus.expireAt.TryConvertToDateTime();
            var remainTimeString = endDate.GetPreciseRemainingString(AppTime.Now, textFormat: StringValueAssetLoader.Instance["event.bonus.activation.remain"]);;
            eventBonusRemainTimeText.text = remainTimeString;
            if (HasRemainTime()) return;
            var isDeadlineExceeded = mFestivalTimeTableCache == null || mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            eventBonusButton.interactable = !isDeadlineExceeded;
            DisposeExitTimeEvent();
            eventBonusRemainTimeRoot.SetActive(false);
        }
        
        private bool HasRemainTime()
        {
            if (FestivalUserStatus.effectStatus == null || string.IsNullOrEmpty(FestivalUserStatus.effectStatus.expireAt)) return false;
            var endDate = FestivalUserStatus.effectStatus.expireAt.TryConvertToDateTime();
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var remainTimeSpan = endDate - AppTime.Now;
            return remainTimeSpan > TimeSpan.Zero;
        }
        
        private bool HasRemainBoostBonusTime()
        {
            if(HasRemainTime() == false)return false;
            
            if (FestivalUserStatus.effectStatus == null || string.IsNullOrEmpty(FestivalUserStatus.effectStatus.expireAt)) return false;
            
            // イベントの自体の期間
            FestivalTimetableMasterObject mTimetable = MasterManager.Instance.festivalTimetableMaster.FindByFestivalId(FestivalUserStatus.mFestivalId);
            DateTime deadline = mTimetable == null ? DateTime.MinValue : mTimetable.deadlineAt.TryConvertToDateTime();
            return deadline.IsPast(AppTime.Now) == false;
        }

        private void DisposeExitTimeEvent()
        {
            if (updateExitTimeDisposable != null)
            {
                updateExitTimeDisposable.Dispose();
                updateExitTimeDisposable = null;
            }
        }
        
        private async UniTask UseFestivalItem()
        {
            var useFestivalItemAPIResponse = await UseFestivalItemAPI();
            if (useFestivalItemAPIResponse == null)
            {
                CruFramework.Logger.LogError("useFestivalItemAPIResponse関連のデータがnullです。　クライアントに確認をお願いします");
                return;
            }
            FestivalUserStatus.effectStatus = useFestivalItemAPIResponse.effectStatus;
            EventManager.Instance.UpdateFestivalEffectStatus(FestivalUserStatus.effectStatus);
            eventBonusButton.interactable = !HasRemainTime(); 
            eventBonusRemainTimeRoot.SetActive(HasRemainBoostBonusTime());
            AddExitTimeEvent();
        }
        
        private void UpdateMissionBadge(long finishedMissionCount, long unreceivedGiftLockedCount, long unopenedGiftBoxCount, string newestGiftLockedAt, MissionCacheData missionCacheData)
        {
            var eventMissionCategoryId = GetEventMissionCategoryId;
            var showBadge = missionCacheData.missionDataDictionary
                .Any(aData => 
                    aData.Key.id == eventMissionCategoryId && aData.Value.Any(aMission => aMission.hasReceivingReward));
            
            //バッジ更新
            FestivalUserStatus.hasMissionNotification = showBadge;
            eventMissionButtonBadge.SetActive(FestivalUserStatus.hasMissionNotification);
            MissionManager.finishedMissionCount = finishedMissionCount;
            LockedItemManager.unreceivedGiftCount = unreceivedGiftLockedCount;
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }

        private bool IsOpenRanking()
        {
            //クラブ未解放
            if( !UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId) ){
                return false;
            }

            //クラブ未所属
            if( UserDataManager.Instance.user.gMasterId == 0 ) {
                return false;
            }

            var festival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            var pointRanking = MasterManager.Instance.pointRankingSettingRealTimeMaster.FindData(FestivalUserStatus.mFestivalId);

            if( pointRanking == null ) {
                return false;
            }

            if( string.IsNullOrEmpty(pointRanking.startAt) || string.IsNullOrEmpty(pointRanking.endAt) ) {
                return false;
            }

            var nowTime = AppTime.Now;
            var startAt = DateTime.Parse(pointRanking.startAt);
            var endAt = DateTime.Parse(pointRanking.endAt);
            if( nowTime < startAt  && endAt < nowTime ) {
                return false;
            }
            
            return true;
        }

        private bool IsOpenGacha()
        {
            var festival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            if( festival == null ) {
                return false;
            }
            if( festival.mGachaSettingId == 0 ) {
                return false;
            }
            return true;
        }

        private void OnOpenEventFinishModal()
        {
            var data = new ConfirmModalData();
            data.Title = StringValueAssetLoader.Instance["event.finish.title"];
            data.Message = StringValueAssetLoader.Instance["event.finish.content"];
            data.NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                window =>
                {
                    eventBonusButton.interactable = false;
                    eventBoostButton.interactable = false;
                    trainingButton.interactable = false;
                    window.Close();
                });
            ConfirmModalWindow.Open(data);
        }

        #region API

        private async UniTask GetFestivalDetailAPI(long mFestivalTimetableId)
        {
            FestivalGetFestivalDetailAPIRequest request = new FestivalGetFestivalDetailAPIRequest();
            FestivalGetFestivalDetailAPIPost post = new FestivalGetFestivalDetailAPIPost();
            post.mFestivalTimetableId = mFestivalTimetableId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            getFestivalDetailAPIResponse = request.GetResponseData();
        }

        private async UniTask<FestivalUseFestivalItemAPIResponse> UseFestivalItemAPI()
        {
            if (getFestivalDetailAPIResponse == null || 
                FestivalUserStatus == null)
            {
                CruFramework.Logger.LogError("getFestivalDetailAPIResponse関連のデータがnullです。　クライアントに確認をお願いします");
                return null;
            }
            
            var mFestivalItem =
                MasterManager.Instance.festivalItemMaster.FindDataByMFestivalTimetableId(this.FestivalUserStatus.mFestivalTimetableId);
            FestivalUseFestivalItemAPIRequest request = new FestivalUseFestivalItemAPIRequest();
            FestivalUseFestivalItemAPIPost post = new FestivalUseFestivalItemAPIPost();
            post.mFestivalItemId = mFestivalItem.id;
            post.value = 1;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();;
        }

        #endregion

        #region EventListeners
        
        public void OnClickActiveEventBonusButton()
        {
            if (getFestivalDetailAPIResponse == null || 
                FestivalUserStatus == null)
            {
                CruFramework.Logger.LogError("getFestivalDetailAPIResponse関連のデータがnullです。　クライアントに確認をお願いします");
                return;
            }

            // 期間チェック
            var isDeadlineExceeded = mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            if (isDeadlineExceeded)
            {
                OnOpenEventFinishModal();
                return;
            }

            var mFestivalItem =
                MasterManager.Instance.festivalItemMaster.FindDataByMFestivalTimetableId(getFestivalDetailAPIResponse
                    .userStatus.mFestivalTimetableId);
            if (mFestivalItem == null)
            {
                CruFramework.Logger.LogError(
                    $"mFestivalItemがnullです mFestivalTimetableId : {FestivalUserStatus.mFestivalTimetableId}");
                return;
            }
            var mFestival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            var pointMaster = MasterManager.Instance.pointMaster.FindData(mFestivalItem.mPointId);
            if (mFestival == null || pointMaster == null) return;
            var cost = mFestivalItem.amount;
            var hasCost = UserDataManager.Instance.point.Find(mFestivalItem.mPointId);
            var value = hasCost?.value ?? 0;
            if (cost > value)
            {
                var data = new CommonExecuteConfirmPointShortageModal.Data();
                data.pointId = mFestivalItem.mPointId;
                data.value = mFestivalItem.amount;
                data.title = StringValueAssetLoader.Instance["common.confirm"];
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CommonExecuteConfirmPointShortage, data);
                return;
            }

            var bonusExpireTimeTotalMinutes = new TimeSpan(0, 0, (int)mFestival.bonusExpireTime);
            var param = new CommonExecuteConfirmModal.Data(
                mFestivalItem.mPointId,
                mFestivalItem.amount,
                StringValueAssetLoader.Instance["event.bonus.activation"],
                string.Format(StringValueAssetLoader.Instance["event.bonus.activation.content"], pointMaster.name,
                    mFestivalItem.amount, pointMaster.unitName, bonusExpireTimeTotalMinutes.TotalMinutes),
                StringValueAssetLoader.Instance["event.bonus.activation.active"], () => UseFestivalItem().Forget());
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CommonExecuteConfirm, param);
        }

        public void OnClickEventHelpButton()
        {
            if (getFestivalDetailAPIResponse == null || 
                FestivalUserStatus == null)
            {
                CruFramework.Logger.LogError("getFestivalDetailAPIResponse関連のデータがnullです。　クライアントに確認をお願いします");
                return;
            }
            var mFestival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            if (mFestival == null) return;
            // Id
            MatchCollection ids = Regex.Matches(mFestival.helpImageIdList, "[0-9]+");
            // 説明
            List<object> descriptions = (List<object>)MiniJSON.Json.Deserialize(mFestival.helpDescriptionList);
            
            HowToPlayModal.HowToData howToData = new HowToPlayModal.HowToData();
            // タイトル
            howToData.Title = StringValueAssetLoader.Instance["event.help"];
            int index = 0;
            foreach(var id in ids)
            {
                // テクスチャアドレスと説明を追加
                howToData.Descriptions.Add(new HowToPlayModal.DescriptionData(PageResourceLoadUtility.GetEventHowToImagePath(id.ToString()), (string)descriptions[index]));
                index++;
            }
            // 遊び方モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HowToPlay, howToData);
        }

        public void OnClickSpecialBoostButton()
        {
            // 期間チェック
            var isDeadlineExceeded = mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            if (isDeadlineExceeded)
            {
                OnOpenEventFinishModal();
                return;
            }
            
            if(boostCharaList == null || boostCharaList.Count == 0) return;
            EventBoostWindow.Open(new EventBoostWindow.WindowParams
            {
                boostCharaList = boostCharaList,
            });
        }
        
        public async void OnClickEventStoryButton()
        {
            var festivalTimetableMasterObject = MasterManager.Instance.festivalTimetableMaster.FindData(getFestivalDetailAPIResponse.userStatus.mFestivalTimetableId);
            var huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindData(festivalTimetableMasterObject.mHuntTimetableId);
            var huntGetTimetableDetailAPIResponse = await StoryManager.Instance.CallHuntGetTimetableDetailApi(huntTimetableMasterObject);
            var huntStageMasterObject = MasterManager.Instance.huntStageMaster.FindStages(huntTimetableMasterObject.mHuntId);
            if (huntStageMasterObject.Length > 0) {
                var firstHuntStageMaster = huntStageMasterObject[0];
                StoryPage.OpenScenarioPage(new StoryScenarioSelectPage.PageParams(firstHuntStageMaster, huntTimetableMasterObject, festivalTimetableMasterObject, FestivalUserStatus, huntGetTimetableDetailAPIResponse.enemyHistory.progress));    
            } else Logger.LogError($"ストーリーデータが見つかりません id:{huntTimetableMasterObject.mHuntId}");
        }

        public void OnClickEventPointRewardButton()
        {
            OpenEventPointModalAsync().Forget();
        }

        private async UniTask OpenEventPointModalAsync()
        {
            var mFestivalTimeTable = MasterManager.Instance.festivalTimetableMaster.FindData(FestivalUserStatus.mFestivalTimetableId);
            if (mFestivalTimeTable.mDailyMissionCategoryIdList.Length < 1) return;
            long missionCategoryId = mFestivalTimeTable.mDailyMissionCategoryIdList[0];
            
            
            var missionCache = await MissionManager.Instance.GetLatestMissionCacheData();
            var dailyMissionCategory = MasterManager.Instance.dailyMissionCategoryMaster.FindData(missionCategoryId);
            if(dailyMissionCategory is null)    return;
            var missionProgressList = missionCache.missionDataDictionary[dailyMissionCategory];


            bool hasUnreceivedReward = missionProgressList.Any(x => x.hasReceivingReward);
            if (hasUnreceivedReward)
            {
                missionCache = await MissionManager.Instance.ReceiveMissionCategory(dailyMissionCategory);
                missionProgressList = missionCache.missionDataDictionary[dailyMissionCategory];
            }
            
            
            EventPointRewardListWindow.Open(new EventPointRewardListWindow.WindowParams
            {
                MissionProgressList = missionProgressList,
                EventPoint = FestivalUserStatus.pointValue,
            });
            //バッジ更新
            FestivalUserStatus.prizeMissionStatus.hasNotification = false;
            eventPointRewardButtonBadge.SetActive(FestivalUserStatus.prizeMissionStatus.hasNotification);
        }
        
        public void OnClickEventRankingButton()
        {
            if( !IsOpenRanking() ) {
                return;
            }
            var festival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            var param = new EventRankingModal.Param();
            param.pointId = festival.mPointId;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.EventRanking, param);
        }

        public void OnClickEventRankingFilter()
        {
            //クラブ未解放
            if( !UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId) ){
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(Pjfb.Club.ClubUtility.clubLockId);
                if( systemLock == null) {
                    return;
                }
                string description = systemLock.description;
                if(string.IsNullOrEmpty(description)){
                    return;
                }
                
                // モーダル
                ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                ConfirmModalData clubLockData = new ConfirmModalData( StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, clubLockData);
                return;
            }

            //クラブ未所属
            if( UserDataManager.Instance.user.gMasterId == 0 ) {
                ConfirmModalData noClubData = new ConfirmModalData(
                    StringValueAssetLoader.Instance["ranking.no_club_ranking_title"],
                    StringValueAssetLoader.Instance["ranking.no_club_ranking_text"],
                    null,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["ranking.no_club_ranking_button"], (window) => {
                        window.Close();
                        AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, false, null);
                    }),
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (window) => {
                        window.Close();
                    })
                );
                
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, noClubData);
                return;
            }

            //ランキング未開催
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["ranking.no_ranking_open_title"],
                StringValueAssetLoader.Instance["ranking.no_ranking_open_text"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (window) => {
                    window.Close();
                })
            );
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
            
        }
        
        
        public void OnClickEventMissionButton()
        {
            MissionManager.Instance.OnClickMissionButton(
                initialTab: MissionTabType.Event,
                initialMissionCategoryId: GetEventMissionCategoryId,
                onReceiveMissionReward: UpdateMissionBadge);
        }
        
        public void OnClickEventGachaButton()
        {
            if ( !IsOpenGacha() ){
                return;
            }

            var festival = MasterManager.Instance.festivalMaster.FindData(FestivalUserStatus.mFestivalId);
            if( festival == null ) {
                return;
            }

            var param = new Pjfb.Gacha.GachaPageArgs();
            param.FocusGachaSettingId = festival.mGachaSettingId;
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Gacha, true, param);
        }
        
        public void OnClickTrainingButton()
        {
            // 期間チェック
            var isDeadlineExceeded = mFestivalTimeTableCache.deadlineAt.TryConvertToDateTime().IsPast(AppTime.Now);
            if (isDeadlineExceeded)
            {
                OnOpenEventFinishModal();
                return;
            }
            
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TrainingPreparation, true, null);
        }

        public void OnClickBackButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
        }

        #endregion

        #region SaveData

        private List<long> confirmedFestivalIdList;
        
        private void SaveConfirmedFestivalId()
        {
            if (confirmedFestivalIdList == null) GetConfirmedFestivalId();
            var beforeSaveDataCount = confirmedFestivalIdList.Count;
            if (confirmedFestivalIdList.Any(id => id == FestivalUserStatus.mFestivalId))
            {
                // 既に確認ずみならスキップ
                return;
            }
            confirmedFestivalIdList.Add(FestivalUserStatus.mFestivalId);
            if (beforeSaveDataCount == confirmedFestivalIdList.Count)
            {
                return;
            }
            var saveString = string.Empty;
            foreach (var id in confirmedFestivalIdList)
            {
                if (!string.IsNullOrEmpty(saveString))
                {
                    saveString += ",";
                }
                saveString += id.ToString();
            }
            LocalSaveManager.saveData.festivalIdString = saveString;
            LocalSaveManager.Instance.SaveData();
        }

        private bool HasNewConfirmedFestival()
        {
            if (confirmedFestivalIdList == null) GetConfirmedFestivalId();
            return !confirmedFestivalIdList?.Any(id => id == FestivalUserStatus.mFestivalId) ?? false;
        }
        
        private void GetConfirmedFestivalId()
        {
            if (confirmedFestivalIdList == null)
            {
                confirmedFestivalIdList = new List<long>();
                var ids = LocalSaveManager.saveData.festivalIdString;
                if (!string.IsNullOrEmpty(ids))
                {
                    var arrayIds = ids.Split(",");
                    foreach (var arrayId in arrayIds)
                    {
                        confirmedFestivalIdList.Add(long.Parse(arrayId));
                    }
                }
            }
        }

        #endregion
    }
}