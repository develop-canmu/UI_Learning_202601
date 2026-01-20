using CruFramework.Page;

namespace Pjfb.LeagueMatchTournament
{
    public enum LeagueMatchTournamentListStatusTabSheetType
    {
        // 開催中
        OnSeasonTournament,
        // 終了済み
        EndSeasonTournament,
    }
    
    // 大会リストの開催状況タブマネージャー
    public class LeagueMatchTournamentListStatusTabSheetManager : SheetManager<LeagueMatchTournamentListStatusTabSheetType>
    {
        
    }
}