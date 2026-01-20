using System.Linq;
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class CommonStoreLotteryCostMasterContainer : MasterContainerBase<CommonStoreLotteryCostMasterObject> {
        long GetDefaultKey(CommonStoreLotteryCostMasterObject masterObject){
            return masterObject.id;
        }
        
        private IEnumerable<CommonStoreLotteryCostMasterObject> GetCommonStoreLotteryCost(long lotteryGroupId)
        {
            return values.Where(data => data.mCommonStoreLotteryCostCategoryId == lotteryGroupId).OrderBy(data => data.stepNumber);
        }
        
        public CommonStoreLotteryCostMasterObject GetCommonStoreLotteryCost(long lotteryGroupId,long updateCount)
        {
            var lotteryCost = GetCommonStoreLotteryCost(lotteryGroupId);
            CommonStoreLotteryCostMasterObject cost = null;
            long updateLimit = 0;
            for (var i = 0; i < lotteryCost.Count(); i++)
            {
                cost = lotteryCost.ElementAt(i);
                if (cost.availableCount == 0)
                {
                    break;
                }
                updateLimit += cost.availableCount;
                if (updateCount < updateLimit)
                {
                    break;
                }
            }
            return cost;
        }
        
        public long GetCommonStoreUpdateLimitByLotteryGroupId(long lotteryGroupId)
        {
            var lotteryCost = GetCommonStoreLotteryCost(lotteryGroupId);
            long updateLimit = 0;
            foreach (var cost in lotteryCost)
            {
                if (cost.availableCount == 0)
                {
                    return 0;
                }
                updateLimit += cost.availableCount;
            }
            return updateLimit;
        }
    }
}
