using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Colosseum;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchResultView : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI headerTitleText = null;
        [SerializeField]
        private ClubEmblemImage opponentClubEmblem = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI opponentClubName = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI resultPointText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI detailPointText = null;
        
        [SerializeField] private GameObject winFlg;
        [SerializeField] private GameObject loseFlg;
        [SerializeField] private GameObject drawFlg;
        
        private GroupLeagueMatchMatchHistory data = null;
        
        public void SetData(GroupLeagueMatchMatchHistory data, ColosseumEventMasterObject mColosseumEvent)
        {
            this.data = data;
            // ヘッダーのタイトル
            if(data.matchType == (int)MatchType.Season)
            {
                // 簡易大会
                if(mColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
                {
                    headerTitleText.text = mColosseumEvent.name + string.Format( StringValueAssetLoader.Instance["league.match_result.title_add"], data.dayNumber); 
                }
                // リーグマッチ
                else
                {
                    headerTitleText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.title_1"], data.dayNumber);
                }
            }
            // 入れ替え戦
            else
            {
                headerTitleText.text = StringValueAssetLoader.Instance["league.match_result.title_2"];
            }
            // 結果
            resultPointText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result"], data.winningPoint, data.winningPointOpponent );
            // 詳細
            detailPointText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result_detail"], data.winCount, data.loseCount, data.drawCount );
            
            // 相手のクラブアイコン
            opponentClubEmblem.SetTexture(data.opponentMGuildEmblemId);            
            // 相手のギルド名
            opponentClubName.text = data.opponentName;
            var result = (BattleResult)data.result;
            
            winFlg.gameObject.SetActive(result == BattleResult.Win);
            loseFlg.gameObject.SetActive(result == BattleResult.Lose);
            drawFlg.gameObject.SetActive(result == BattleResult.None);
        }
        
        /// <summary>uGUI</summary>
        public void OnLongTapClub()
        {
            LeagueMatchUtility.OpenClubInfo(data.groupId, data.groupType).Forget();
        }
    }
}