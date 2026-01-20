using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Utility;

namespace Pjfb.Common
{
    public class MissionModal : ModalWindow
    {

        #region InnerClass
        public class Parameters
        {
            public MissionCacheData missionCacheData;
            public MissionTabType initialTab = MissionTabType.Daily;
            public float initialScroll = 1f;
            public long initialMissionCategoryId = 0;
            public Action<long, long, long, string, MissionCacheData> onReceiveMissionReward; // ホーム画面の表示を更新させる用
            public Action onWindowClosed = null;
            
            public Parameters(MissionCacheData missionCacheData, MissionTabType initialTab, long initialMissionCategoryId, Action<long, long, long, string, MissionCacheData> onReceiveMissionReward, Action onWindowClosed = null)
            {
                this.missionCacheData = missionCacheData;
                this.initialTab = initialTab;
                this.initialMissionCategoryId = initialMissionCategoryId;
                this.onReceiveMissionReward = onReceiveMissionReward;
                this.onWindowClosed = onWindowClosed;
            }

            public Parameters SetInitialValue(MissionTabType initialTab, long initialMissionCategoryId, float initialScroll)
            {
                this.initialTab = initialTab;
                this.initialMissionCategoryId = initialMissionCategoryId;
                this.initialScroll = initialScroll;
                return this;
            }

            public void SetMissionCacheData(MissionCacheData missionCacheData)
            {
                this.missionCacheData = missionCacheData;
            }
        }
        
        public class MissionSheetOpenParams
        {
            public float initialScroll;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private GameObject touchGuard;
        [SerializeField] private MissionSheetManager sheetManager;
        [SerializeField] private MissionSheetDaily dailySheet;
        [SerializeField] private MissionSheetTotal totalSheet;
        [SerializeField] private MissionSheetEvent eventSheet;
        [SerializeField] private MissionSheetSwitchButton dailySheetTab;
        [SerializeField] private MissionSheetSwitchButton totalSheetTab;
        [SerializeField] private MissionSheetSwitchButton eventSheetTab;
        [SerializeField] private MissionModalEventPage eventPage;
        [SerializeField] private TMPro.TMP_Text balloonSerifText;
        #endregion

        #region PrivateFields
        private Parameters _parameters;
        private MissionTabType _currentTabType;
        private long _showingMissionCategoryId;
        private HashSet<long> _rewardReceivedMissionCategoryIds = new();
        #endregion

        #region PublicStaticMethods
        public static void Open(Parameters parameters)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Mission, parameters);
        }
        #endregion
        
        #region OverrideMethods
        protected override void OnAwake()
        {
            sheetManager.OnPreOpenSheet += OnPreOpenSheet;
            base.OnAwake();
        }
        
        private void OnDestroy()
        {
            sheetManager.OnPreOpenSheet -= OnPreOpenSheet;
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _parameters = (Parameters) args;
            _rewardReceivedMissionCategoryIds = new HashSet<long>();
            InitDisplay(isForceOpen: false);
            await base.OnPreOpen(args, token);
        }
        #endregion

        #region PrivateMethods
        private void InitDisplay(bool isForceOpen)
        {
            Init();
            SetDisplay();
            OpenInitialPage(isForceOpen);
        }
        
        private async void OpenInitialPage(bool isForceOpen)
        {
            var initialScrollValue = _parameters.initialScroll;
            if (_parameters.initialTab == MissionTabType.Event && _parameters.initialMissionCategoryId > 0) {
                foreach (var keyValuePair in eventSheet.parameters.eventMissionDataDictionary) {
                    if (keyValuePair.Key.id != _parameters.initialMissionCategoryId) continue;
                    
                    OpenEventPage(
                        missionEventData: keyValuePair.Key,
                        missionProgressPairDataList: keyValuePair.Value,
                        useFade: false,
                        initialScrollValue: initialScrollValue);
                    initialScrollValue = 1f;
                    break;
                }
            }

            await sheetManager.OpenSheetAsync(_parameters.initialTab, args: new MissionSheetOpenParams{initialScroll = initialScrollValue}, isForceOpen: isForceOpen);
        }
        
        private void RefreshDisplay(MissionCacheData missionCacheData, float initialScrollValue)
        {
            _parameters.SetMissionCacheData(missionCacheData);
            _parameters.SetInitialValue(initialTab: _currentTabType, initialMissionCategoryId: _showingMissionCategoryId, initialScroll: initialScrollValue);
            InitDisplay(isForceOpen: true);
        }

        private void OpenEventPage(DailyMissionCategoryMasterObject missionEventData, List<MissionProgressPairData> missionProgressPairDataList, bool useFade, float initialScrollValue = 1f)
        {
            _showingMissionCategoryId = missionEventData.id;
            eventPage.SetDisplay(new MissionModalEventPage.Parameters
            {
                missionEventData = missionEventData,
                missionProgressPairDataList = missionProgressPairDataList,
                initalScrollValue = initialScrollValue,
                onListItemClickChallengeButton = OnClickChallengeButtonListItem,
                onListItemClickReceiveButton = OnClickReceiveButtonListItem,
                onClickReceiveAllButton = OnClickReceiveAllEvent,
                onClickBackButton = OnClickEventPageBackButton,
            }, useFade: useFade);
        }
        
        private void Init()
        {
            eventPage.Init();
            dailySheet.Init();
            totalSheet.Init();
            eventSheet.Init();
            dailySheetTab.SetBadge(isActive: false);
            totalSheetTab.SetBadge(isActive: false);
            eventSheetTab.SetBadge(isActive: false);
            balloonSerifText.text = string.Empty;
        }

        private void SetDisplay()
        {
            var now = AppTime.Now;
            var uTagObj = UserDataManager.Instance.tagObj.ToDictionaryOfList(aData => aData.adminTagId);
            var missionDataDictionary = _parameters.missionCacheData.missionDataDictionary
                .Except(aData =>
                    aData.Key.challengeConditionTypeEnum == MissionChallengeConditionType.TagId && // MissionChallengeConditionType.TagIdに対して、有効になっているtagObjがない場合は表示から排除する
                    !(uTagObj.TryGetValue(aData.Key.challengeConditionValue, out var tagObj) && tagObj.Any(aTagObj => aTagObj.expireAt.TryConvertToDateTime().IsFuture(now))));
            var dailyMissionDataDictionary = missionDataDictionary
                .Where(aData => aData.Key.tabType == MissionTabType.Daily)
                .ToDictionary(aData => aData.Key, aData => aData.Value);
            dailySheetTab.SetBadge(isActive: dailyMissionDataDictionary.SelectMany(aData => aData.Value).Any(aData => aData.hasReceivingReward));
            dailySheet.SetDisplay(new MissionSheetDaily.Parameters{
                dailyMissionDataDictionary = dailyMissionDataDictionary,
                onListItemClickReceiveButton = OnClickReceiveButtonListItem,
                onListItemClickChallengeButton = OnClickChallengeButtonListItem,
                onClickReceiveAllButton = OnClickReceiveAllEvent,
            });

            dailyMissionDataDictionary = missionDataDictionary
                .Where(aData => aData.Key.tabType == MissionTabType.Total)
                .ToDictionary(aData => aData.Key, aData => aData.Value);
            totalSheetTab.SetBadge(isActive: dailyMissionDataDictionary.SelectMany(aData => aData.Value).Any(aData => aData.hasReceivingReward));
            totalSheet.SetDisplay(new MissionSheetTotal.Parameters{
                dailyMissionDataDictionary = dailyMissionDataDictionary,
                onListItemClickReceiveButton = OnClickReceiveButtonListItem,
                onListItemClickChallengeButton = OnClickChallengeButtonListItem,
                onClickReceiveAllButton = OnClickReceiveAllEvent,
            });

            dailyMissionDataDictionary = missionDataDictionary
                .Where(aData => aData.Key.tabType == MissionTabType.Event)
                .Except(aData => 
                    aData.Key.displayType == (long)MissionCategoryDisplayType.HideCompleted && 
                    aData.Value.TrueForAll(aMissionProgress => aMissionProgress.completed) &&
                    !_rewardReceivedMissionCategoryIds.Contains(aData.Key.id)) // 報酬受け取った直後はしばらく表示する仕様
                .ToDictionary(aData => aData.Key, aData => aData.Value);
            eventSheetTab.SetBadge(isActive: dailyMissionDataDictionary.SelectMany(aData => aData.Value).Any(aData => aData.hasReceivingReward));
            eventSheet.SetDisplay(new MissionSheetEvent.Parameters{
                eventMissionDataDictionary = dailyMissionDataDictionary,
                onListItemClickEventButton = OnClickEventListItem,
                onClickReceiveAllButton = OnClickReceiveAllEvent,
            });
        }

        private void UpdateBalloonSerif()
        {
            balloonSerifText.text = _currentTabType switch {
                MissionTabType.Daily => "日々の積み重ねが重要よ！",
                MissionTabType.Total => "これまでの努力の賜物ね！\nこれからも頑張りましょう♪",
                MissionTabType.Event => "期間限定で開催中よ！\n確認を忘れないでね",
                _ => string.Empty
            };
        }
        #endregion

        #region EventListener
        private void OnPreOpenSheet(MissionTabType tabType)
        {
            _currentTabType = tabType;
            UpdateBalloonSerif();
        }

        public void OnClickCloseButton()
        {
            if (!sheetManager.CanOpenSheet) return;
            Close(onCompleted: _parameters.onWindowClosed);
        }

        private void OnClickChallengeButtonListItem(MissionModalListItem.ItemParams itemParams)
        {
            if (!sheetManager.CanOpenSheet) return;
            ServerActionCommandUtility.ProceedAction(nullableCommandParam: itemParams.missionProgressPairData.missionData.linkEx);
        }
        
        private async void OnClickReceiveButtonListItem(MissionModalListItem.ItemParams itemParams, DateTime receiveEndAt, float initialScrollValue)
        {
            // サポート器具上限チェック
            var prizeJsonData = PrizeJsonUtility.GetPrizeContainerData(itemParams.missionProgressPairData.missionData.prizeJson[0]);
            if (prizeJsonData.itemIconType == ItemIconType.SupportEquipment) 
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }
            touchGuard.SetActive(true);
            var missionCacheData = await MissionManager.Instance.OnClickReceiveButtonListItem(itemParams.missionProgressPairData, receiveEndAt, _parameters.onReceiveMissionReward);
            // MissionModalが閉じられている場合のみnullで帰ってくるので後の処理を行わない
            if (missionCacheData == null) return;
            touchGuard.SetActive(false);
            _rewardReceivedMissionCategoryIds.Add(itemParams.missionProgressPairData.missionCategory.id);
            RefreshDisplay(missionCacheData, initialScrollValue);
        }
        
        private void OnClickEventListItem(MissionModalEventListItem.ItemParams itemParams)
        {
            OpenEventPage(itemParams.missionEventData, itemParams.missionProgressPairDataList, useFade: true);
        }
        
        private async void OnClickReceiveAllEvent(List<DailyMissionCategoryMasterObject> missionCategoryDataList, List<List<MissionProgressPairData>> missionProgressPairDataList, DateTime receiveEndAt, float initialScrollValue)
        {
            if (!sheetManager.CanOpenSheet) return;
            
            // サポート器具上限チェック
            var isContainingSupportEquipment = false;
            foreach (var missionProgressPairDatas in missionProgressPairDataList)
            {
                foreach (var data in missionProgressPairDatas)
                {
                    var prizeJsonData = PrizeJsonUtility.GetPrizeContainerData(data.missionData.prizeJson[0]);
                    if (data.hasReceivingReward && prizeJsonData.itemIconType == ItemIconType.SupportEquipment)
                    {
                        isContainingSupportEquipment = true;
                        break;
                    }
                }
            }
            if (isContainingSupportEquipment)
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }
            touchGuard.SetActive(true);
            var missionCacheData = await MissionManager.Instance.OnClickReceiveAllButton(missionCategoryDataList, receiveEndAt, _parameters.onReceiveMissionReward);
            // MissionModalが閉じられている場合のみnullで帰ってくるので後の処理を行わない
            if (missionCacheData == null) return;
            touchGuard.SetActive(false);
            missionCategoryDataList
                .Select(aMissionCategory => aMissionCategory.id)
                .ForEach(aMissionCategoryId => _rewardReceivedMissionCategoryIds.Add(aMissionCategoryId));
            RefreshDisplay(missionCacheData, initialScrollValue);
        }
        
        private async void OnClickEventPageBackButton(MissionModalEventPage eventPage)
        {
            _showingMissionCategoryId = 0;
            eventSheet.UpdateDisplay(initialScrollValue: eventSheet.scrollValue).Forget();
            await eventPage.ShowFade(active: false);
        }
        #endregion
    }
}
