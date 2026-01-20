using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using CruFramework.Page;
using CruFramework.Utils;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Pjfb.Extensions;
using Pjfb.Home;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Common
{
    public class MissionManager : Singleton<MissionManager>
    {
        #region バッジ用
        public static long finishedMissionCount;
        #endregion
        
        #region PlayerPrefs
        private const string ShownFirstBeginnerMissionPopup = "ShownFirstBeginnerMissionPopup";
        public static bool ShownFirstBeginnerMissionPopupPlayerPrefs
        {
            get => ObscuredPrefs.Get<int>(key: ShownFirstBeginnerMissionPopup, defaultValue: 0) == 1;
            set => ObscuredPrefs.Set<int>(key: ShownFirstBeginnerMissionPopup, value ? 1 : 0);
        }
        #endregion
    
        #region PrivateProperties
        [CanBeNull] private MissionCacheData missionCacheData;
        #endregion
        
        #region PublicMethods
        public async void OnClickMissionButton(Action<long, long, long, string, MissionCacheData> onReceiveMissionReward, MissionTabType initialTab = MissionTabType.Daily, long initialMissionCategoryId = 0, Action onWindowClosed = null)
        {
            var missionCache = await GetLatestMissionCacheData();
            MissionModal.Open(new MissionModal.Parameters(missionCache, initialTab, initialMissionCategoryId, onReceiveMissionReward, onWindowClosed));
        }
        
        /// <param name="missionProgressPairData">「受け取る」の対象項目データ</param>
        /// <param name="receiveEndAt">デイリーの場合はstep計算、それ以外はmissionCategory.receiveEndAt</param>
        public async UniTask<MissionCacheData> OnClickReceiveButtonListItem(MissionProgressPairData missionProgressPairData, DateTime receiveEndAt, Action<long, long, long, string, MissionCacheData> onReceiveMissionReward)
        {
            if (receiveEndAt.IsPast(AppTime.Now)) {
                await ShowScheduleEndedModal();
                ClearCache();
                return await GetLatestMissionCacheData();
            }
            
            var missionBase = missionProgressPairData.nullableMissionProgressData;
            var response = await CallMissionReceiveAPI(missionBase);
            missionCacheData?.UpdateMissionDateDictionary(response.missionList);
            
            var modalWindow = await RewardModal.OpenAsync(new RewardModal.Parameters (prizeJsonWraps: response.prizeJsonList, null), ModalOptions.KeepFrontModal);
            
            // ボタンで閉じた場合はtrueが帰ってくる
            if ((bool)await modalWindow.WaitCloseAsync())
            {
                await ShowAutoSellConfirmModal(response.autoSell);
                
                var finishedMissionCount = missionCacheData.GetFinishedMissionCount();
                onReceiveMissionReward?.Invoke(finishedMissionCount, response.unreceivedGiftLockedCount, response.unopenedGiftBoxCount, response.newestGiftLockedAt, missionCacheData);

                return missionCacheData;
            }
            
            // ボタン以外で閉じた場合はnullを返す
            return null;
        }
        
        /// <param name="missionCategories">「一括受け取る」の対象ミッションカテゴリー</param>
        /// <param name="receiveEndAt">デイリーの場合はstep計算、それ以外はmissionCategory.receiveEndAt</param>
        public async UniTask<MissionCacheData> OnClickReceiveAllButton(List<DailyMissionCategoryMasterObject> missionCategories, DateTime receiveEndAt, Action<long, long, long, string, MissionCacheData> onReceiveMissionReward)
        {
            if (receiveEndAt.IsPast(AppTime.Now)) {
                await ShowScheduleEndedModal();
                ClearCache();
                return await GetLatestMissionCacheData();
            }

            var responseDictionary = await CallMissionReceiveCategoryAPI(missionCategories);
            var missionList = responseDictionary.SelectMany(aData => aData.Value.missionList);
            missionCacheData?.UpdateMissionDateDictionary(missionList);

            var modalWindow = await RewardModal.OpenAsync(new RewardModal.Parameters (prizeJsonWraps: responseDictionary.SelectMany(aResponsePair => aResponsePair.Value.prizeJsonList).ToList(), null), ModalOptions.KeepFrontModal);
            
            // ボタンで閉じた場合はtrueが帰ってくる
            if ((bool)await modalWindow.WaitCloseAsync())
            { 
                var autoSellList = responseDictionary.Values.Select(x => x.autoSell).ToList();
                await ShowAutoSellConfirmModal(autoSellList);

                var finishedMissionCount = missionCacheData.GetFinishedMissionCount();
                var lastResponse = responseDictionary.Last().Value;
                onReceiveMissionReward?.Invoke(finishedMissionCount, lastResponse.unreceivedGiftLockedCount, lastResponse.unopenedGiftBoxCount, lastResponse.newestGiftLockedAt, missionCacheData);
                return missionCacheData;
            }
            
            // ボタン以外で閉じた場合はnullを返す
            return null;
        }

        public void ShowBeginnerMissionModal (HomeBeginnerMissionButton.Parameters parameters, Action onReturned = null)
        {
            HomeBeginnerMissionClearPopupWindow.Open(new HomeBeginnerMissionClearPopupWindow.Parameters
            {
                beginnerMissionObject =parameters.beginnerMissionObject,
                beginnerMissionCategoryStatus = parameters.beginnerMissionCategoryStatus,
                onComplete = (_) => onReturned?.Invoke()
            });
        }

        public async void ReceiveBeginnerMission(HomeBeginnerMissionButton.Parameters parameters, Action<MissionCategoryStatus> onFinish)
        {
            var response = await CallMissionReceiveAPI(parameters.beginnerMissionCategoryStatus.targetMission);
            var clearModalWindow = await HomeBeginnerMissionClearModalWindow.OpenAsync();
            await clearModalWindow.WaitCloseAsync();
            
            var modalWindow = await RewardModal.OpenAsync(new RewardModal.Parameters (prizeJsonWraps: response.prizeJsonList, null), ModalOptions.KeepFrontModal);
            await modalWindow.WaitCloseAsync();
            
            onFinish?.Invoke(response.missionCategoryStatus);
        }

        public void TryShowBeginnerMissionModal(MissionCategoryStatus beginnerMissionCategoryStatus, Action nextAction)
        {
            if (beginnerMissionCategoryStatus == null || ShownFirstBeginnerMissionPopupPlayerPrefs) nextAction?.Invoke();
            else {
                ShownFirstBeginnerMissionPopupPlayerPrefs = true;
                
                var parameter = new HomeBeginnerMissionButton.Parameters(beginnerMissionCategoryStatus);
                if (parameter.hasActiveBeginnerMission && parameter.beginnerMissionCategoryStatus.targetMission.progressStatus != (long)MissionProgressStatus.ReceivingReward) ShowBeginnerMissionModal(parameter, onReturned: nextAction);
                else nextAction?.Invoke(); // 初心者ミッション全クリア状態で端末引き継ぎを行った際は一回だけここに入ります
            }
        }
        
        public async UniTask<MissionCacheData> ReceiveMissionCategory(DailyMissionCategoryMasterObject missionCategory, string buttonStringKey = "common.close")
        {
            var responseDictionary = await CallMissionReceiveCategoryAPI(new List<DailyMissionCategoryMasterObject>{missionCategory});
            var missionList = responseDictionary.SelectMany(aData => aData.Value.missionList);
            missionCacheData?.UpdateMissionDateDictionary(missionList);

            var modalWindow = await RewardModal.OpenAsync(new RewardModal.Parameters (prizeJsonWraps: responseDictionary.SelectMany(aResponsePair => aResponsePair.Value.prizeJsonList).ToList(), null, buttonStringKey), ModalOptions.KeepFrontModal);
            await modalWindow.WaitCloseAsync();
            var autoSellList = responseDictionary.Values.Select(x => x.autoSell).ToList();
            await ShowAutoSellConfirmModal(autoSellList);
            
            return missionCacheData;
        }
        
        
        
        public async UniTask<MissionCacheData> GetLatestMissionCacheData(bool forceUpdate = false)
        {
            var now = AppTime.Now;
            var isLatestCache = missionCacheData != null && missionCacheData.createTimeStamp.IsSameMinute(now);
            if (!forceUpdate && isLatestCache) return missionCacheData;
            
            var response = await CallMissionGetListAPI();
            missionCacheData = new MissionCacheData(response, MasterManager.Instance, createTimeStamp: now);
        
            return missionCacheData;
        }
        
        public void ClearCache()
        {
            missionCacheData = null;
        }

        public async void OnCommunitySendYellFinished()
        {
            if (missionCacheData == null) {
                missionCacheData = await GetLatestMissionCacheData();
                finishedMissionCount = missionCacheData.GetFinishedMissionCount();
            } else {
                var hasProgressingYellMission = missionCacheData.missionDataDictionary.Values
                    .SelectMany(aData => aData).ToList()
                    .Exists(aData => aData.stillProgressing && aData.missionData.linkEx.StartsWith("openPage:follow")); // openPage:followはイエールを送るミッションの判定
                
                // サーバ負担が増やさないため、仮でfinishedMissionCount値を足す
                // ホーム画面へ遷移する際にホームAPIで正式なデータが反映される
                if (hasProgressingYellMission) finishedMissionCount++;     
            }
            
            HomeTopPage.TryUpdateMissionBadge(finishedMissionCount, missionCacheData);
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();            
            ClearCache();
        }
        #endregion
        
        #region Call API Methods
        private async UniTask<MissionGetListAPIResponse> CallMissionGetListAPI()
        {
            var request = new MissionGetListAPIRequest();
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        private async UniTask<MissionReceiveAPIResponse> CallMissionReceiveAPI(MissionUserAndGuild missionProgressData)
        {
            var request = new MissionReceiveAPIRequest();
            request.SetPostData(new MissionReceiveAPIPost{uDailyMissionId = missionProgressData.id});
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        /// <summary>
        /// データとデザイン設計的に一つの一覧に複数カテゴリが含める考えになっているため、Call実行する際に複数カテゴリを対応可能にしてます
        /// </summary>
        private async UniTask<Dictionary<DailyMissionCategoryMasterObject, MissionReceiveCategoryAPIResponse>> CallMissionReceiveCategoryAPI(List<DailyMissionCategoryMasterObject> missionCategoryDataList)
        {
            var retVal = new Dictionary<DailyMissionCategoryMasterObject, MissionReceiveCategoryAPIResponse>();
            foreach(var category in missionCategoryDataList)
            {
                var request = new MissionReceiveCategoryAPIRequest();
                request.SetPostData(new MissionReceiveCategoryAPIPost {mDailyMissionCategoryId = category.id});
                await APIManager.Instance.Connect(request);
                retVal.Add(category, request.GetResponseData());
            }
        
            return retVal;
        }
        #endregion

        #region PrivateMethods
        private async UniTask ShowScheduleEndedModal()
        { 
            var showingConfirmWindow = true;
            ConfirmModalWindow.Open(
                data: new ConfirmModalData(
                    title: StringValueAssetLoader.Instance["common.confirm"],
                    message: "受け取り期限を過ぎています", 
                    caution: string.Empty,
                    buttonParams: new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], 
                        onClick: modal => modal.Close(onCompleted: () => showingConfirmWindow = false)
                    )), ModalOptions.KeepFrontModal);

            await UniTask.WaitWhile(() => showingConfirmWindow);
        }

        private async UniTask ShowAutoSellConfirmModal(NativeApiAutoSell autoSell, Action onFinish = null)
        {
            await ShowAutoSellConfirmModal(new List<NativeApiAutoSell> { autoSell }, onFinish);
        }
        
        private async UniTask ShowAutoSellConfirmModal(List<NativeApiAutoSell> autoSellList, Action onFinish = null)
        {
            autoSellList = autoSellList?.Where(x =>
                x.prizeListGot is not null && x.prizeListSold is not null &&
                (x.prizeListGot.Length > 0 || x.prizeListSold.Length > 0)).ToList();
            // 自動売却があるかどうかチェック
            if (autoSellList is not null && autoSellList.Count > 0)
            {
                var autoSellModalData = new AutoSellConfirmModal.Data(autoSellList);
                var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoSellConfirm, autoSellModalData);
                await modal.WaitCloseAsync();
            }
            onFinish?.Invoke();
        }
        #endregion
        
        #region OverrideMethods
        protected override void Init()
        {
            ClearCache();
            base.Init();
        }
        #endregion
    }

    public class MissionCacheData
    {
        public Dictionary<DailyMissionCategoryMasterObject, List<MissionProgressPairData>> missionDataDictionary = new();
        public DateTime createTimeStamp;
        
        public MissionCacheData(MissionGetListAPIResponse missionGetListAPIResponse, MasterManager masterManagerInstance, DateTime createTimeStamp)
        {
            this.createTimeStamp = createTimeStamp;
            GenerateMissionDataDictionary(missionGetListAPIResponse, masterManagerInstance);
        }
    
        private void GenerateMissionDataDictionary(MissionGetListAPIResponse missionGetListAPIResponse, MasterManager masterManagerInstance)
        {
            var playerMissionData = missionGetListAPIResponse.missionList.ToDictionary(aMission => aMission.mDailyMissionId);
            var masterMission = masterManagerInstance.dailyMissionMaster.values;
            List<DailyMissionCategoryMasterObject> masterMissionCategory = new List<DailyMissionCategoryMasterObject>();// メモ：id:1(Daily), 2(Total), >=3(Events)
            
            // 表示するミッションを選定する
            foreach (DailyMissionCategoryMasterObject masterObject in masterManagerInstance.dailyMissionCategoryMaster.values)
            {
                // tagListForReceiveがある場合は、タグを所持しているかどうかを確認する
                if (masterObject.tagListForReceive == null || masterObject.tagListForReceive.Length == 0)
                {
                    masterMissionCategory.Add(masterObject);
                }
                else
                {
                    foreach (long tagId in masterObject.tagListForReceive)
                    {
                        if (UserDataManager.Instance.tag.Contains(tagId) == true)
                        {
                            masterMissionCategory.Add(masterObject);
                            break;
                        }
                    }
                }
            }
            
            var now = AppTime.Now;
            missionDataDictionary = masterMissionCategory
                .Where(aMissionCategory => aMissionCategory.startAt.TryConvertToDateTime().IsPast(now) &&
                                           aMissionCategory.receiveEndAt.TryConvertToDateTime().IsFuture(now))
                .ToDictionary(
                    keySelector: aData => aData,
                    elementSelector: aData => masterMission
                        .Where(aMission => aMission.mDailyMissionCategoryId == aData.id)
                        .Select(aMission => new MissionProgressPairData (
                            missionCategory: aData,
                            missionData: aMission,
                            nullableMissionProgressData: playerMissionData.TryGetValue(aMission.id, out var result) ? result : null))
                        .ToList());
        }
        
        public void UpdateMissionDateDictionary(IEnumerable<MissionUserAndGuild> missionBases)
        {
            var missionProgressPairDictionary = missionDataDictionary
                .SelectMany(aMissionCategoryPair => aMissionCategoryPair.Value)
                .GroupBy(aMissionProgressPair => aMissionProgressPair.missionData.id)
                .ToDictionary(aMissionProgressPair => aMissionProgressPair.Key, aMissionProgressPair => aMissionProgressPair.ToList());
            foreach (var aMissionBase in missionBases) {
                if (missionProgressPairDictionary.TryGetValue(aMissionBase.mDailyMissionId, out var aMissionPairList)) {
                    aMissionPairList.ForEach(adata => adata.SetMissionProgressData(aMissionBase));
                }
            }
        }

        public int GetFinishedMissionCount()
        {
            return missionDataDictionary.Values.SelectMany(aData => aData).Count(aData => aData.hasReceivingReward);
        }
    }
    
    public class MissionProgressPairData
    {
        public DailyMissionCategoryMasterObject missionCategory;
        public DailyMissionMasterObject missionData;
        public MissionUserAndGuild nullableMissionProgressData;

        private const long StatusSortOrder = 10000000;
        public float sortOrder => Mathf.Abs(((nullableMissionProgressData?.progressStatus ?? (long)MissionProgressStatus.Progressing) - 2) * 3 + 1) * StatusSortOrder + missionData.sortNumber;
        public bool hasReceivingReward => nullableMissionProgressData?.progressStatus == (int)MissionProgressStatus.ReceivingReward;
        public bool stillProgressing => nullableMissionProgressData == null || nullableMissionProgressData.progressStatus == (int)MissionProgressStatus.Progressing;
        public bool completed => nullableMissionProgressData?.progressStatus == (int)MissionProgressStatus.End;

        public MissionProgressStatus MissionProgressStatus => nullableMissionProgressData == null
            ? MissionProgressStatus.Progressing
            : (MissionProgressStatus)nullableMissionProgressData.progressStatus;
        public void SetMissionProgressData(MissionUserAndGuild missionProgressData)
        {
            nullableMissionProgressData = missionProgressData;
        }

        public bool ShowMission(Dictionary<long, MissionProgressPairData> missionDataDictionary)
        {
            // 1.progressStatusが「未達成」以外は全て表示する
            if (nullableMissionProgressData != null && nullableMissionProgressData.progressStatus != (int) MissionProgressStatus.Progressing) return true;
            // 2.previousミッションがない場合は表示する
            if (missionData.previousMDailyMissionId == 0) return true;
            // 3.previousミッションがある前提、previousミッションのprogressStatusが「報酬受け取り済み」の場合は表示する
            if (missionDataDictionary.TryGetValue(missionData.previousMDailyMissionId, out var previousMissionData) && previousMissionData.nullableMissionProgressData is {progressStatus: 3}) return true;
            
            return false;
        }
    
        public MissionProgressPairData(DailyMissionCategoryMasterObject missionCategory, DailyMissionMasterObject missionData, MissionUserAndGuild nullableMissionProgressData)
        {
            this.missionCategory = missionCategory;
            this.missionData = missionData;
            this.nullableMissionProgressData = nullableMissionProgressData;
        }
    }
    
    public enum MissionProgressStatus
    {
        Progressing = 1,
        ReceivingReward  = 2,
        End    = 3,
        Unknown     = 99
    }

    public enum MissionCategoryDisplayType
    {
        UnDeclared = 0,
        ShowAll = 1,
        HideCompleted = 2,
    }
}