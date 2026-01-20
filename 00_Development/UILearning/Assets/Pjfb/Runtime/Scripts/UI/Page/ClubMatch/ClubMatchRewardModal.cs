using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Colosseum;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.ClubMatch
{
    public enum ColosseumRewardCauseType
    {
        PersonalReward = 1,
        GuildPersonalReward,
        PersonalScoreReward,
        GuildReward, 
    }
    
    public class ClubMatchRewardModal : ColosseumRewardModal
    {
        [SerializeField] private ClubMatchStatusView clubMatchStatusView;
        private ColosseumSeasonData colosseumSeasonData;
        private ColosseumRankingGroup colosseumRankingGroup;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (args != null)
            {
                colosseumSeasonData = (ColosseumSeasonData)args;
            }

            var groupId = colosseumSeasonData.UserSeasonStatus.groupSeasonStatus.groupId;
            colosseumRankingGroup = colosseumSeasonData.BattleRankingGroups.FirstOrDefault(group => group.groupId == groupId);
            
            long gradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(colosseumSeasonData.MColosseumEvent.mColosseumGradeGroupId, groupId);
            await clubMatchStatusView.SetClubViewAsync(colosseumRankingGroup, gradeNumber);
        }
        
        protected override void UpdateView()
        {
            
            if (colosseumSeasonData == null)
            {
                return;
            }

            var ranking = colosseumRankingGroup?.ranking;

            var rewardList =
                MasterManager.Instance.colosseumRankingPrizeMaster.GetColosseumRankingPrize(
                    colosseumSeasonData.MColosseumEvent.mColosseumRankingPrizeGroupId, colosseumSeasonData.UserSeasonStatus.gradeNumber,
                    (long)ColosseumRewardCauseType.GuildPersonalReward);

            var rankingRewardList = new List<RewardScrollData>();
            foreach (var reward in rewardList)
            {
                var rankingReward = new RewardScrollData();
                rankingReward.rankTop = reward.rankTop;
                rankingReward.rankBottom = reward.rankBottom;
                rankingReward.myRank = reward.rankTop <= ranking && reward.rankBottom >= ranking;
                rankingReward.prizeList = reward.prizeJson;
                rankingRewardList.Add(rankingReward);
            }
            scroller.SetItems(rankingRewardList);
        }
    }
}