

using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Ranking
{
    public class RankingCharacterPowerTabSheetClubTab : RankingCharacterPowerTabSheet
    {
        [SerializeField]
        private RankingOwnAffiliate ownAffiliate = null;
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField]
        private GameObject annotationText = null;
        
        private RankingGetGuildCommonRankListAPIResponse response = null;
        
        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            response = (RankingGetGuildCommonRankListAPIResponse)rankingData;

            ownAffiliate.gameObject.SetActive(true);
            if (UserDataManager.Instance.user.gMasterId == 0)
            {
                ownAffiliate.NoJoinClub();
            }
            else
            {
                ownAffiliate.JoinClub();
                ownAffiliate.SetMyClubInfo(
                    response.ranking.mGuildEmblemId,
                    response.ranking.myRank,
                    response.ranking.guildName,
                    new BigValue(response.ranking.myValue),
                    false,
                    isOmissionPoint
                );   
            }
            
            RankingClubScrollItem.Param[] rankingList = new RankingClubScrollItem.Param[response.ranking.guildRankList.Length];
            
            annotationText.SetActive(response.ranking.guildRankList.Length == 0);
            
            for (int i = 0; i < response.ranking.guildRankList.Length; i++)
            {
                RankGuild rank = response.ranking.guildRankList[i];
                rankingList[i] = new RankingClubScrollItem.Param(
                    rank.mGuildEmblemId,
                    rank.id,
                    rank.rank,
                    rank.name,
                    new BigValue(rank.value),
                    rank.id == UserDataManager.Instance.user.gMasterId,
                    isOmissionPoint
                );
            }
            scrollGrid.SetItems(rankingList);
        }
        
        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            rankingRewardView.SetView(response.ranking.rankingPrizeList, response.ranking.myRank, RankingAffiliateTabSheetType.Club);
        }
    }
}