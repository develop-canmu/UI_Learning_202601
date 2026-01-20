using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.ClubMatch
{
    public class ClubMatchMatchingPage : Page
    {
        
        public class Data : PageData
        {
            public ColosseumState clubMatchState;
            
            public Data(ColosseumSeasonData seasonData)
            {
                targetPage = ClubMatchPageType.ClubMatchMatching;
                if (seasonData == null) return;
                SeasonData = seasonData;
                clubMatchState = ColosseumManager.GetColosseumState(seasonData.SeasonHome);
            }
            
            public Data(ColosseumSeasonData seasonData, PageType pageType) : this(seasonData)
            {
                callerPage = pageType;
            }
        }
        
        #region SerializeFields
        [SerializeField] private PoolListContainer rankingContainer;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text totalPowerText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private UIButton updateButton;
        #endregion

        // private ColosseumRankingUser[] rankingUserList;
        // private ColosseumEventMasterObject mColosseumEvent;
        //
        private Data currentArgs;
        private ClubData clubData;

        #region OverrideMethods
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentArgs = (Data)args;
            if (currentArgs.SeasonData == null)
            {
                return;
            }
            var userSeasonStatus = currentArgs.SeasonData.UserSeasonStatus;
            
            clubData = await ClubMatchUtility.GetClubData();
            var user = UserDataManager.Instance.user;
            nameText.text = user.name;
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], userSeasonStatus.ranking);
            totalPowerText.text = user.maxCombatPower.Value.ToDisplayString(omissionTextSetter.GetOmissionData());
            deckRankImage.SetTexture(user.maxDeckRank.Value);
            
            UpdateRankingView().Forget();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            if (currentArgs.SeasonData == null)
            {
                gameObject.SetActive(false);
                ClubMatchPage.ShowErrorDialog(() =>
                {
                    AppManager.Instance.UIManager.PageManager.PrevPage();
                }, msgBodyKey:"common.error.season_not_found");
            }
        }

        #endregion

        #region PrivateMethods
        
        
        private async UniTask UpdateRankingView()
        {
            var season = currentArgs.SeasonData.UserSeasonStatus;
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],season.ranking);
            
            updateButton.interactable = false;
            
            var gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(
                currentArgs.SeasonData.MColosseumEvent.mColosseumGradeGroupId, season.gradeNumber);
            
            var rankingUserList = await ColosseumManager.RequestGetRankingAsync(season.sColosseumEventId,gradeMaster.roomCapacity * ConfigManager.Instance.maxGuildMemberCount);
            
            var data = new List<ClubMatchRankingItem.RankingData>();
            if (rankingUserList == null || rankingUserList.Length == 0)
            {
                return;
            }

            foreach (var user in rankingUserList)
            {
                var isSelf = user.uMasterId.Equals(currentArgs.SeasonData.UserSeasonStatus.uMasterId);
                var isSameClub = user.groupId.Equals(currentArgs.SeasonData.UserSeasonStatus.groupSeasonStatus.groupId) &&
                                 user.groupType.Equals(currentArgs.SeasonData.UserSeasonStatus.groupSeasonStatus.groupType);
                data.Add(new ClubMatchRankingItem.RankingData
                {
                    userData = user,colosseumSeasonData = currentArgs.SeasonData, disableOnClickAction = isSameClub, borderLineType = BorderLineType.Non, scoreType = ScoreType.CurrentScore,
                    OnSizeChanged = rankingContainer.RefreshView,
                    backArgs = new Data(currentArgs.SeasonData)
                });
                if (isSelf)
                {
                    BigValue clubMatchTotalCombatPower = ColosseumManager.GetClubMatchTotalCombatPower(user.defenseCount, new BigValue(user.combatPower));
                    totalPowerText.text = clubMatchTotalCombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
                    deckRankImage.SetTexture(StatusUtility.GetPartyRank(clubMatchTotalCombatPower));
                    
                }
            }
            
            rankingContainer.SetDataList(data,slideIn:false).Forget();
            
            // 1度更新かけたあとは5秒間updateできない
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            
            if (updateButton != null)
            {
                updateButton.interactable = true;
            }
        }
        
        #endregion
        //
        #region EventListeners
        
        public void OnClickBack(UIButton sender)
        {
            ClubMatchPage.OpenPage(false,new ClubMatchTopPage.Data(currentArgs.SeasonData){callerPage = PageType.Home});
        }
        
        public void OnClickUpdateUserList()
        {
            UpdateRankingView().Forget();
        }
        
        #endregion
    }
}