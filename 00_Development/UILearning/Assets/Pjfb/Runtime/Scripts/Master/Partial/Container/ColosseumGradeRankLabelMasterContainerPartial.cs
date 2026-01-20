
using System.Linq;

namespace Pjfb.Master {

    public partial class ColosseumGradeRankLabelMasterContainer : MasterContainerBase<ColosseumGradeRankLabelMasterObject> {
        long GetDefaultKey(ColosseumGradeRankLabelMasterObject masterObject){
            return masterObject.id;
        }

        public int GetRankNumber(long mColosseumGradeGroupId, long gradeNumber, long ranking)
        {
            // mColosseumGradeGroupIdでシーズンごとにランク設定変更できるのでid = rankNumberとせずにrank設定数から算出する
            var gradeRankLabelList = values.Where(gradeRankLabel => gradeRankLabel.mColosseumGradeGroupId == mColosseumGradeGroupId)
                .OrderByDescending(gradeRankLabel => gradeRankLabel.id);
            var rank = 0;
            foreach (var gradeRankLabel in gradeRankLabelList)
            {
                rank++;
                if (gradeRankLabel.gradeNumber == gradeNumber &&
                    gradeRankLabel.rankTop <= ranking &&
                    gradeRankLabel.rankBottom >= ranking)
                {
                    break;
                }
            }
            return rank;
        }
    }
}
