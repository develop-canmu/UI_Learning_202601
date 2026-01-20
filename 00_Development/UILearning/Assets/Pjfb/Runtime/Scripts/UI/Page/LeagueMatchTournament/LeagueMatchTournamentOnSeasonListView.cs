using System.Collections.Generic;
using System.Linq;
using Pjfb.LeagueMatch;
using Pjfb.Master;

namespace Pjfb.LeagueMatchTournament
{
    // 開催中大会リストビュー
    public class LeagueMatchTournamentOnSeasonListView : LeagueMatchTournamentListBaseView
    {
        protected override void SetItem(List<LeagueMatchTournamentInfo> matchInfoList)
        {
            List<LeagueMatchTournamentBannerScrollDynamicItem.Param> itemList = new List<LeagueMatchTournamentBannerScrollDynamicItem.Param>();

            // バナーグループ分類毎のリストデータ
            Dictionary<LeagueMatchTournamentManager.BannerGroupType, List<LeagueMatchTournamentInfo>> groupTournamentList = new Dictionary<LeagueMatchTournamentManager.BannerGroupType, List<LeagueMatchTournamentInfo>>();
            
            // グループ毎に分類
            foreach (LeagueMatchTournamentInfo tournamentInfo in matchInfoList)
            {
                if (groupTournamentList.TryGetValue(tournamentInfo.GroupType, out List<LeagueMatchTournamentInfo> groupInfoList) == false)
                {
                    groupInfoList = new List<LeagueMatchTournamentInfo>();
                    groupTournamentList.Add(tournamentInfo.GroupType, groupInfoList);
                }
                groupInfoList.Add(tournamentInfo);
            }
            
            // グループ毎のListを並び替え
            foreach (LeagueMatchTournamentManager.BannerGroupType groupType in groupTournamentList.Keys.ToList())
            {
                groupTournamentList[groupType] = SortTournamentDataList(groupTournamentList[groupType]);
            }

            var sortGroupDataList = groupTournamentList.OrderBy(x =>
            {
                ColosseumEventGroupLabelMasterObject data = MasterManager.Instance.colosseumEventGroupLabelMaster.FindData((int)x.Key);
                // 定義されていないならエラーを出す
                if (data == null)
                {
                    CruFramework.Logger.LogError($"ColosseumEventGroupLabelMasterに{x.Key}が存在しません");
                    return -1;
                }
                
                return data.displayPriority;
            });
            
            // グループ順に並び替え(GroupType昇順)
            foreach (KeyValuePair<LeagueMatchTournamentManager.BannerGroupType, List<LeagueMatchTournamentInfo>> pair in sortGroupDataList)
            {
                itemList.Add(new LeagueMatchTournamentBannerScrollDynamicItem.Param(pair.Key, pair.Value));
            }
            scroll.SetItems(itemList);
        }
    }
}