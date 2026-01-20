using System.Collections.Generic;
using Pjfb.LeagueMatch;

namespace Pjfb.LeagueMatchTournament
{
    // 終了済み大会リスト
    public class LeagueMatchTournamentEndSeasonListView : LeagueMatchTournamentListBaseView
    {
        protected override void SetItem(List<LeagueMatchTournamentInfo> tournamentInfos)
        {
            List<LeagueMatchTournamentBannerScrollDynamicItem.Param> itemList = new List<LeagueMatchTournamentBannerScrollDynamicItem.Param>();
            // 終了済み大会データをセット
            itemList.Add(new LeagueMatchTournamentBannerScrollDynamicItem.Param(LeagueMatchTournamentManager.BannerGroupType.None, SortTournamentDataList(tournamentInfos)));

            scroll.SetItems(itemList);
        }
    }
}