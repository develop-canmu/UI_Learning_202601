
using System.Linq;

namespace Pjfb.Master {

    public partial class BillingRewardMasterContainer : MasterContainerBase<BillingRewardMasterObject> {
        long GetDefaultKey(BillingRewardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
