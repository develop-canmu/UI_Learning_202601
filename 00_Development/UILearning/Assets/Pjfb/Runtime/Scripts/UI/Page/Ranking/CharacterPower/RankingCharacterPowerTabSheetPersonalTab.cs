
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Ranking
{
    // 選手戦力個人タブ
    public class RankingCharacterPowerTabSheetPersonalTab : RankingCharacterPowerTabSheet
    {
        
        [SerializeField]
        private RankingUserInfoView userInfoView = null;
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField] 
        private GameObject annotationText = null;
        
        private RankingGetUserCommonRankListAPIResponse response = null;
        
        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            // レスポンス
            response = (RankingGetUserCommonRankListAPIResponse)rankingData;
            
            // 自身のランキング情報を表示
            // 選手戦力のヘッダー内の順位をセットする
            userInfoView.SetHeaderRank(response.ranking.myRank);
            // ランキングなし
            if(response.ranking.myChara == null || response.ranking.myChara.mCharaId == 0)
            {
                userInfoView.ShowEmptyCharacterPower(true);
            }
            else
            {
                userInfoView.ShowEmptyCharacterPower(false);
                // キャラId
                userInfoView.SetCharacter( response.ranking.myChara.mCharaId, new BigValue(response.ranking.myChara.combatPower), response.ranking.myChara.charaRank );
                // シナリオ
                userInfoView.SetScenario( response.ranking.myChara.mTrainingScenarioId );
            }

            // ユーザー名
            userInfoView.SetName( UserDataManager.Instance.user.name );
            
            //////////// ユーザーのランキングリスト
            
            // ランキングリストを生成
            RankingUserScrollItem.Arguments[] rankingList = new RankingUserScrollItem.Arguments[response.ranking.charaRankList.Length];
            
            annotationText.SetActive(response.ranking.charaRankList.Length == 0);
            
            for(int i=0;i<rankingList.Length;i++)
            {
                RankCharaVariable rank = response.ranking.charaRankList[i];
                rankingList[i] = new RankingUserScrollItem.Arguments(
                    rank.id,
                    rank.name,
                    rank.mCharaId,
                    new BigValue(rank.combatPower),
                    rank.charaRank,
                    rank.rank,
                    rank.mTrainingScenarioId
                    );
            }
            
            // スクロールに登録
            scrollGrid.SetItems(rankingList);
        }

        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            rankingRewardView.SetView(response.ranking.rankingPrizeList, response.ranking.myRank, RankingAffiliateTabSheetType.Personal);
        }
    }
}