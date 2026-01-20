using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using CruFramework.UI;
using System.Linq;
using Pjfb.UI;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Colosseum;
using Pjfb.Networking.App.Request;

namespace Pjfb.ClubMatch
{
    [Serializable]
    public class PersonalRankingSheet
    {
        public ClubMatchPersonalRankingTabSheetType type;
        public PoolListContainer listContainer;
        [HideInInspector] public bool isInitialized;
    }

    public class ClubMatchPersonalRankingModal : ModalWindow
    {
        [SerializeField] private ClubMatchStatusView clubMatchStatusView;
        [SerializeField] private ClubMatchPersonalRankingTabSheetManager tabSheetManager;
        [SerializeField] private PersonalRankingSheet[] personalRankingSheetList;
        [SerializeField] private UIButton updateButton;
        
        private ColosseumSeasonData currentColosseumSeasonData;
        private List<ColosseumRankingUser> rankingUserListOriginal, sortedRankingUserList;
        private bool isScrollerInitialized;
        private long selfCumulativeRanking;
        private BigValue currentTotalCombatPower = BigValue.Zero;

        public static void Open(ColosseumSeasonData colosseumSeasonData)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchPersonalRanking, colosseumSeasonData);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentColosseumSeasonData = (ColosseumSeasonData)args;
            await InitRankingApi();
            clubMatchStatusView.SetPersonalView(currentColosseumSeasonData.UserSeasonStatus, selfCumulativeRanking, currentTotalCombatPower);
            tabSheetManager.OnPreOpenSheet -= InitializeScroller;
            tabSheetManager.OnPreOpenSheet += InitializeScroller;
            updateButton.SetClickIntervalTimer(updateButton.ClickTriggerInterval);
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            if (!isScrollerInitialized)
            {
                InitializeScroller(tabSheetManager.CurrentSheetType);
            }
        }

        private async UniTask InitRankingApi()
        {
            // データ設定
            var season = currentColosseumSeasonData.UserSeasonStatus;
            if (season == null)
            {
                return;
            }

            var gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(
                currentColosseumSeasonData.MColosseumEvent.mColosseumGradeGroupId, season.gradeNumber);
            
            rankingUserListOriginal = (await ColosseumManager.RequestGetRankingAsync(season.sColosseumEventId,gradeMaster.roomCapacity * ConfigManager.Instance.maxGuildMemberCount)).ToList();
            sortedRankingUserList = rankingUserListOriginal.OrderByDescending(data => data.score).ToList();
            sortedRankingUserList.ForEach(user => 
            {
                user.scoreRanking = sortedRankingUserList.IndexOf(user) + 1; 
                if (user.uMasterId == currentColosseumSeasonData.UserSeasonStatus.uMasterId)
                {
                    selfCumulativeRanking = user.scoreRanking;
                    currentTotalCombatPower = new BigValue(user.combatPower);
                }
            });
        }

        private void InitializeScroller(ClubMatchPersonalRankingTabSheetType type)
        {
            clubMatchStatusView.SetPersonalView(currentColosseumSeasonData.UserSeasonStatus, selfCumulativeRanking, currentTotalCombatPower);

            // Sheet管理
            var personalRankingSheet = personalRankingSheetList.FirstOrDefault(sheet => sheet.type == type);
            if (personalRankingSheet == null || personalRankingSheet.isInitialized)
            {
                return;
            }
            
            var data = new List<ClubMatchRankingItem.RankingData>();
            if (rankingUserListOriginal == null || rankingUserListOriginal.Count == 0)
            {
                return;
            }

            var rankingUserList = type == ClubMatchPersonalRankingTabSheetType.Score ? sortedRankingUserList : rankingUserListOriginal;

            foreach (var user in rankingUserList)
            {
                var isSameClub = user.groupId.Equals(currentColosseumSeasonData.UserSeasonStatus.groupSeasonStatus.groupId) &&
                                 user.groupType.Equals(currentColosseumSeasonData.UserSeasonStatus.groupSeasonStatus.groupType);
                data.Add(new ClubMatchRankingItem.RankingData
                {
                    userData = user,colosseumSeasonData = currentColosseumSeasonData, disableOnClickAction = isSameClub, borderLineType = BorderLineType.Non, scoreType = (ScoreType)type,
                    OnSizeChanged = personalRankingSheet.listContainer.RefreshView,
                    backArgs = new ClubMatchTopPage.Data(currentColosseumSeasonData)
                });
            }

            // 表示
            personalRankingSheet.listContainer.SetDataList(data,slideIn:false).Forget();
            personalRankingSheet.isInitialized = true;

            isScrollerInitialized = true;
        }

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        
        public async void OnClickUpdateUserList()
        {
            await InitRankingApi();
            isScrollerInitialized = false;
            foreach (var personalRankingSheet in personalRankingSheetList)
            {
                personalRankingSheet.isInitialized = false;
            }
            InitializeScroller(tabSheetManager.CurrentSheetType);
        }
        #endregion
    }
}
