
using System.Linq;

namespace Pjfb.Master {

    public partial class CombinationProgressMasterContainer : MasterContainerBase<CombinationProgressMasterObject> {
        long GetDefaultKey(CombinationProgressMasterObject masterObject){
            return masterObject.id;
        }

        public CombinationProgressMasterObject FindDataByGroupIdIdAndProgressLevel(long mCombinationProgressGroupId, long progressLevel)
        {
            var mCombinationProgress = values.FirstOrDefault(data => data.mCombinationProgressGroupId == mCombinationProgressGroupId && data.level == progressLevel);
            if (mCombinationProgress == null)
            {
                CruFramework.Logger.LogError($"not find data  groupId : {mCombinationProgressGroupId} progressLevel : {progressLevel}");
            }
            return mCombinationProgress;
        }
    }
}
