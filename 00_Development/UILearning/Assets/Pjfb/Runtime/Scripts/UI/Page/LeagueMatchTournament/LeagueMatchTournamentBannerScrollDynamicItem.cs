using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.LeagueMatch;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.LeagueMatchTournament
{
    public class LeagueMatchTournamentBannerScrollDynamicItem : ScrollDynamicItem
    {
        public class Param
        {
            private LeagueMatchTournamentManager.BannerGroupType groupType;
            public LeagueMatchTournamentManager.BannerGroupType GroupType => groupType;
            
            private List<LeagueMatchTournamentInfo> tournamentInfoList = null;
            public List<LeagueMatchTournamentInfo> TournamentInfoList => tournamentInfoList;

            public Param(LeagueMatchTournamentManager.BannerGroupType groupType, List<LeagueMatchTournamentInfo> tournamentInfoList)
            {
                this.groupType = groupType;
                this.tournamentInfoList = tournamentInfoList;
            }
        }

        // バナーグループView
        [SerializeField]
        private LeagueMatchTournamentBannerGroupView groupView = null;
        
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            groupView.SetView(param.GroupType, param.TournamentInfoList);
            // レイアウト再計算
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
        }
    }
}