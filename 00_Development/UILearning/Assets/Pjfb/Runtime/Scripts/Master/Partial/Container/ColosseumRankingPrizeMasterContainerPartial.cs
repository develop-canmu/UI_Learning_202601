using System.Collections.Generic;
using System.Linq;
using CruFramework;

namespace Pjfb.Master {

    //（1 => 個人ランキング報酬、 2 => ギルド順位準拠の個人向け報酬、 3 => 個人スコアランキング報酬）、4 => ギルド順位基準で付与される、対ギルド報酬（g_masterに紐づく報酬を指定）、5 => ギルド順位基準で付与されるで付与される、ギルドポイント・ギルド経験値指定（prizeは無視されます）
    public enum ColosseumRankingPrizeCauseType
    {
        PersonalRanking = 1,
        PersonalGuildRanking = 2,
        PersonalScore = 3,
        GuildRanking = 4,
        GuildPoint = 5,
    }
    
    public partial class ColosseumRankingPrizeMasterContainer : MasterContainerBase<ColosseumRankingPrizeMasterObject> {
        long GetDefaultKey(ColosseumRankingPrizeMasterObject masterObject){
            return masterObject.id;
        }

        public List<ColosseumRankingPrizeMasterObject> GetColosseumRankingPrize(long mColosseumRankingPrizeGroupId, long groupNumber)
        {
            return values.Where(data => data.mColosseumRankingPrizeGroupId == mColosseumRankingPrizeGroupId &&
                                        data.gradeNumber == groupNumber).OrderBy(data => data.rankTop).ToList();
        }
        
        public List<ColosseumRankingPrizeMasterObject> GetColosseumRankingPrize(long mColosseumRankingPrizeGroupId, long groupNumber, long causeType)
        {
            return GetColosseumRankingPrize(mColosseumRankingPrizeGroupId, groupNumber)
                .Where(value => value.causeType == causeType).ToList();
        }
        
        public PrizeJsonWrap[] GetRankingPrizeJson(long mColosseumRankingPrizeGroupId, long gradeNumber,ColosseumRankingPrizeCauseType causeType,long rank)
        {
            return GetRankingPrizeJson(mColosseumRankingPrizeGroupId, gradeNumber, (long)causeType, rank);
        }
        
        public PrizeJsonWrap[] GetRankingPrizeJson(long mColosseumRankingPrizeGroupId, long gradeNumber,long causeType,long rank)
        {
            return values.FirstOrDefault(data
                => data.mColosseumRankingPrizeGroupId == mColosseumRankingPrizeGroupId &&
                   data.causeType == causeType &&
                   data.gradeNumber == gradeNumber &&
                   data.rankTop <= rank &&
                   data.rankBottom >= rank)?.prizeJson;
        }
    }
}
