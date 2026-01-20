using CruFramework.Page;

namespace Pjfb.ClubMatch
{
    public enum ClubMatchRewardTabSheetType
    {
        Club,       //クラブランキング報酬
        Personal,   //個人ランキング報酬
        Score,      //累計スコア報酬
    }
    
    public class ClubMatchRewardTabSheetManager : SheetManager<ClubMatchRewardTabSheetType>
    {
        
    }
}