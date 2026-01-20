using System.Linq;

namespace Pjfb.Master {

    public partial class PointRankingSettingRealTimeMasterContainer : MasterContainerBase<PointRankingSettingRealTimeMasterObject> {
        long GetDefaultKey(PointRankingSettingRealTimeMasterObject masterObject){
            return masterObject.id;
        }

        PointRankingSettingRealTimeMasterObject FindByPointId(long pointId){
            return values.FirstOrDefault( itr => itr.mPointId == pointId );
        }
    }
}
