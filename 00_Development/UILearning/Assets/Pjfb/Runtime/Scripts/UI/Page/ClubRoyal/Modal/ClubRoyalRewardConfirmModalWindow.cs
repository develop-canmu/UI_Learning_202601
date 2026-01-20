using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalRewardConfirmModalWindow : ModalWindow
    {
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField] 
        private ClubRoyalClubInfoView clubInfoView = null;
        
        private ColosseumSeasonData colosseumSeasonData;
        private GroupLeagueMatchGroupStatusDetail guildGroupStatusDetail = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            ClubRoyalMenuModalWindow.ClubRoyalInfo clubRoyalInfo = (ClubRoyalMenuModalWindow.ClubRoyalInfo)args;
            colosseumSeasonData = clubRoyalInfo.MatchInfo.SeasonData;
            guildGroupStatusDetail = clubRoyalInfo.GroupStatusDetail;
            UpdateView();
            SetClubInfo();
            
            return base.OnPreOpen(args, token);
        }

        private void UpdateView()
        {
            List<ColosseumRankingPrizeMasterObject> rewardList = 
                MasterManager.Instance.colosseumRankingPrizeMaster.GetColosseumRankingPrize(
                    colosseumSeasonData.MColosseumEvent.mColosseumRankingPrizeGroupId,
                    colosseumSeasonData.UserSeasonStatus.gradeNumber
                );
            
            ClubRoyalRewardConfirmScrollItem.Param[] rankingRewardList = new ClubRoyalRewardConfirmScrollItem.Param[rewardList.Count];
            for (int i = 0; i < rewardList.Count; i++)
            {
                ColosseumRankingPrizeMasterObject reward = rewardList[i];
                rankingRewardList[i] = new ClubRoyalRewardConfirmScrollItem.Param(
                    reward.rankTop,
                    reward.rankBottom,
                    reward.prizeJson,
                    reward.rankTop <= guildGroupStatusDetail.ranking && reward.rankBottom >= guildGroupStatusDetail.ranking
                    
                );
            }
            scrollGrid.SetItems(rankingRewardList);
        }
        
        private void SetClubInfo()
        {
            clubInfoView.SetRank(guildGroupStatusDetail.ranking);
            clubInfoView.SetEmblem(guildGroupStatusDetail.mGuildEmblemId);
            clubInfoView.SetName(guildGroupStatusDetail.name);
        }

        public void OnClickCloseButton()
        {
            Close();
        }
    }
}