
using System.Linq;

namespace Pjfb.Master {

    public partial class CombinationTrainingStatusMasterContainer : MasterContainerBase<CombinationTrainingStatusMasterObject> {
        long GetDefaultKey(CombinationTrainingStatusMasterObject masterObject){
            return masterObject.id;
        }

        public CombinationTrainingStatusMasterObject FindDataByCombinationIdAndProgressLevel(long mCombinationId, long progressLevel)
        {
            var mCombinationTrainingStatus = values.FirstOrDefault(data => data.mCombinationId == mCombinationId && data.progressLevel == progressLevel);
            if (mCombinationTrainingStatus == null)
            {
                CruFramework.Logger.LogError($"not find data  mCombinationId : {mCombinationId} progressLevel : {progressLevel}");
            }
            return mCombinationTrainingStatus;
        }
    }
}
