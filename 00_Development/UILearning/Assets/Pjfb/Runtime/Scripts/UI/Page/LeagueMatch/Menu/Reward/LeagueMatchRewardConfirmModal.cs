using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.EventRanking;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRewardConfirmModal : ModalWindow
    {
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField]
        private LeagueMatchRewardClubInfoView clubView = null;
        [SerializeField]
        private GameObject clubViewRoot = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            LeagueMatchMenuModal.Arguments arguments = (LeagueMatchMenuModal.Arguments)args;

            // 報酬リスト取得
            List<ColosseumRankingPrizeMasterObject> rewardList =  MasterManager.Instance.colosseumRankingPrizeMaster.GetColosseumRankingPrize(arguments.MColosseumEvent.mColosseumRankingPrizeGroupId, arguments.SeasonData.UserSeasonStatus.gradeNumber);
            
            long ranking = arguments.SeasonData.UserSeasonStatus.groupSeasonStatus.ranking;

            List<RewardScrollData> rankingRewardList = new List<RewardScrollData>();
            foreach(ColosseumRankingPrizeMasterObject reward in rewardList)
            {
                RewardScrollData rankingReward = new RewardScrollData();
                rankingReward.rankTop = reward.rankTop;
                rankingReward.rankBottom = reward.rankBottom;
                rankingReward.myRank = reward.rankTop <= ranking && reward.rankBottom >= ranking;
                rankingReward.prizeList = reward.prizeJson;
                rankingRewardList.Add(rankingReward);
            }
            scrollGrid.SetItems(rankingRewardList);
            
            if(ranking > 0)
            {
                // アクティブ化
                clubViewRoot.SetActive(true);
                // クラブ名
                clubView.SetName(arguments.SeasonData.UserSeasonStatus.groupSeasonStatus.name);
                // クラブエンブレム
                clubView.SetEmblem(arguments.SeasonData.UserSeasonStatus.groupSeasonStatus.mGuildEmblemId);
                // ランキング
                clubView.SetRanking(ranking);
            }
            else
            {
                // 非アクティブ化
                clubViewRoot.SetActive(false);
            }
            
            await base.OnPreOpen(args, token);
        }
    }
}