using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Ranking;
using Pjfb.UserData;

namespace Pjfb
{
    /// <summary>総戦力、選手戦力、ポイントのクラブランキング画面が同じため共通シートで対応</summary>
    public class RankingClubTab : RankingTabSheet
    {
        /// <summary>レスポンスのキャッシュ</summary>
        private RankingGetGuildCommonRankListAPIResponse response = null;
        
        /// <summary>自身のクラブ情報を表示するフッター</summary>
        [SerializeField]
        private RankingOwnAffiliate rankingOwnAffiliate = null;
        
        /// <summary>スクロール</summary>
        [SerializeField]
        private ScrollGrid scrollGrid = null;

        /// <summary>ランキング情報がない場合にアクティブにするオブジェクト</summary>
        [SerializeField]
        private GameObject annotationTextObj = null;

        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            // レスポンスの型にキャスト
            response = (RankingGetGuildCommonRankListAPIResponse)rankingData;
            
            // 自身のクラブ情報をセットする
            // 自身のクラブ情報を表示するオブジェクトの親をアクティブにする
            rankingOwnAffiliate.gameObject.SetActive(true);
            
            // 自身がクラブに所属しているのかを判定する。0がクラブ未所属
            if (UserDataManager.Instance.user.gMasterId == 0)
            {
                // 所属していない場合
                rankingOwnAffiliate.NoJoinClub();
            }
            else
            {
                // 所属している場合
                rankingOwnAffiliate.JoinClub();
                // 自身のクラブ情報をセットする
                rankingOwnAffiliate.SetMyClubInfo(
                    response.ranking.mGuildEmblemId,
                    response.ranking.myRank,
                    response.ranking.guildName,
                    new BigValue(response.ranking.myValue),
                    false,
                    isOmissionPoint
                );   
            }
            
            // クラブランキングのスクロール領域で表示するリストアイテムのパラメータを作成する
            RankingClubScrollItem.Param[] clubRankingDataArray = new RankingClubScrollItem.Param[response.ranking.guildRankList.Length];
            
            // ランキング情報がない場合は、その旨を示すアノテーションを表示する
            annotationTextObj.SetActive(response.ranking.guildRankList.Length == 0);
            
            for (int i = 0; i < response.ranking.guildRankList.Length; i++)
            {
                RankingClubScrollItem.Param data = new RankingClubScrollItem.Param(
                    response.ranking.guildRankList[i].mGuildEmblemId,
                    response.ranking.guildRankList[i].id,
                    response.ranking.guildRankList[i].rank,
                    response.ranking.guildRankList[i].name,
                    new BigValue(response.ranking.guildRankList[i].value),
                    // 特定の順位におけるclubIdと自身のクラブIdが一致するか一致する場合はハイライト表示
                    response.ranking.guildRankList[i].id == UserDataManager.Instance.user.gMasterId,
                    isOmissionPoint
                );
                
                clubRankingDataArray[i] = data;
            }
            
            // スクロールにアイテムをセット
            scrollGrid.SetItems(clubRankingDataArray);
        }
        
        /// <summary>個人、クラブタブ切替時に報酬データをセットする</summary>
        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            rankingRewardView.SetView(response.ranking.rankingPrizeList, response.ranking.myRank, RankingAffiliateTabSheetType.Club);
        }
    }
}