
using System.Linq;

namespace Pjfb.Master {

    public partial class CostPointEscalationMasterContainer : MasterContainerBase<CostPointEscalationMasterObject> {
        long GetDefaultKey(CostPointEscalationMasterObject masterObject){
            return masterObject.id;
        }

        public CostPointEscalationMasterObject GetCostPoint(long mCostPointEscalationGroupId,long battleCountExtra)
        {
            var costList = values.Where(value => value.mCostPointEscalationGroupId == mCostPointEscalationGroupId).OrderBy(value => value.timesMax);

            foreach (var cost in costList)
            {
                if (battleCountExtra < cost.timesMax)
                {
                    return cost;
                }
            }
            // 該当コストなし 挑戦不可
            return null;
        }
    }
}
