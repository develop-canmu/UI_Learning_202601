
using System.Linq;

namespace Pjfb.Master {

    public partial class ColosseumScoreBattleStayCorrectionMasterContainer : MasterContainerBase<ColosseumScoreBattleStayCorrectionMasterObject> 
    {
        long GetDefaultKey(ColosseumScoreBattleStayCorrectionMasterObject masterObject) 
        {
            return masterObject.id;
        }

        public long GetAdditionRate(long refValue,int type)
        {
            var valueObj = values.OrderBy(data => data.threshold).LastOrDefault(value => value.type == type && value.threshold <= refValue);
            return (valueObj != null) ? valueObj.additionRate : 0;
        }
    }
}
