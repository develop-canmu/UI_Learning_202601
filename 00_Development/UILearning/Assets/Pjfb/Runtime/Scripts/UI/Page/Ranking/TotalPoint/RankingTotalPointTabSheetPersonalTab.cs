using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Ranking
{
    public class RankingTotalPointTabSheetPersonalTab : RankingTotalPointTabSheet
    {
        [SerializeField]
        private RankingOwnAffiliate ownAffiliate = null;
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField]
        private GameObject annotationText = null;
        
        private RankingGetUserCommonRankListAPIResponse response = null;
        
        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            response = (RankingGetUserCommonRankListAPIResponse)rankingData;
            
            ownAffiliate.gameObject.SetActive(true);
            ownAffiliate.SetMyInfo(
                UserDataManager.Instance.user.uMasterId,
                response.ranking.myRank,
                UserDataManager.Instance.user.mIconId,
                UserDataManager.Instance.user.name,
                new BigValue(response.ranking.myValue),
                false,
                isOmissionPoint
            );
            
            RankingOwnScrollItem.Param[] rankingList = new RankingOwnScrollItem.Param[response.ranking.pointRankList.Length];
            
            annotationText.SetActive(response.ranking.pointRankList.Length == 0);

            for (int i = 0; i < response.ranking.pointRankList.Length; i++)
            {
                RankUser rank = response.ranking.pointRankList[i];
                rankingList[i] = new RankingOwnScrollItem.Param(
                    rank.id,
                    rank.rank,
                    rank.mIconId,
                    rank.name,
                    new BigValue(rank.value),
                    rank.id == UserDataManager.Instance.user.uMasterId,
                    isOmissionPoint
                    );
            }
            scrollGrid.SetItems(rankingList);
        }
        
        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            rankingRewardView.SetView(response.ranking.rankingPrizeList, response.ranking.myRank, RankingAffiliateTabSheetType.Personal);
        }
    }
}