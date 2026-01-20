using CruFramework.Page;

namespace Pjfb.ClubMatch
{
    public enum ClubMatchPersonalRankingTabSheetType
    {
        Personal = 0,   //個人ランキング報酬
        Score = 1,      //累計スコア報酬
    }
    
    public class ClubMatchPersonalRankingTabSheetManager : SheetManager<ClubMatchPersonalRankingTabSheetType>
    {
        
    }
}