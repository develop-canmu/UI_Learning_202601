using Pjfb.ClubMatch;
using Pjfb.UserData;
using UnityEngine;
using TMPro;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchTopHeaderTeamInfo : MonoBehaviour
    {
        [SerializeField] private ClubEmblemImage teamClubIcon;
        [SerializeField] private ClubRankImage rankIcon = null;
        [SerializeField] private GameObject rankIconDummy = null;
        [SerializeField] private TextMeshProUGUI teamClubName;
        [SerializeField] private TMP_Text position;
        [SerializeField] private GameObject winFlg;
        [SerializeField] private GameObject loseFlg;
        [SerializeField] private GameObject greyMask;
        
        private long groupType;
        private long groupId;
        
        public async void IniUI(ColosseumClientHandlingType clientHandlingType, long groupId, long groupType, long mGuildEmblemId, string clubName, long ranking, BattleResult result, Progress leagueMatchProgress, long gradeNumber = -1)
        {
            this.groupId = groupId;
            this.groupType = groupType;
            
            await teamClubIcon.SetTextureAsync(mGuildEmblemId);
            if (gradeNumber <= -1)
            {
                rankIcon.gameObject.SetActive(false);
                rankIconDummy.SetActive(false);
            }
            else
            {
                // ダミー画像
                rankIconDummy.SetActive(clientHandlingType == ColosseumClientHandlingType.InstantTournament);
                // ランク画像
                rankIcon.gameObject.SetActive(clientHandlingType != ColosseumClientHandlingType.InstantTournament);
                if(clientHandlingType != ColosseumClientHandlingType.InstantTournament)
                {
                    await rankIcon.SetTextureAsync(gradeNumber);
                }
            }
            
            teamClubName.text = clubName;
            position.gameObject.SetActive(ranking != -1);
            position.text = string.Format( StringValueAssetLoader.Instance["league.match_result.ranking"], ranking);
            winFlg.SetActive(result == BattleResult.Win);
            loseFlg.SetActive(result == BattleResult.Lose);
            greyMask.SetActive(result == BattleResult.Lose);
        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public async void OnLongTap()
        {
            await LeagueMatchUtility.OpenClubInfo(groupId, groupType);
        }
    }
}