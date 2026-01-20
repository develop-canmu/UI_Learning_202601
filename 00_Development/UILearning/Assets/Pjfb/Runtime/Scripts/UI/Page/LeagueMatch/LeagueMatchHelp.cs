using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Colosseum;
using Pjfb.LeagueMatch;
using TMPro;

namespace Pjfb
{
    public class LeagueMatchHelp : MonoBehaviour
    {
        [SerializeField]
        private UIHelp help = null;

        // ヘルプタイトル
        [SerializeField]
        private TMP_Text helpTitle = null;

        // リーグマッチヘルプキー
        [StringValue]
        [SerializeField]
        private string leagueMatchHelp = string.Empty;
        
        // リーグマッチ大会ヘルプキー
        [StringValue]
        [SerializeField]
        private string leagueMatchTournamentHelp = string.Empty;
        
        public void SetHelp(LeagueMatchInfo matchInfo)
        {
            // 大会なら大会用ヘルプを設定
            if (matchInfo.MColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
            {
                help.SetHelpCategory(leagueMatchTournamentHelp);
                helpTitle.text = StringValueAssetLoader.Instance[leagueMatchTournamentHelp];
            }
            else
            {
                help.SetHelpCategory(leagueMatchHelp);
                helpTitle.text = StringValueAssetLoader.Instance[leagueMatchHelp];
            }
        }
    }
}