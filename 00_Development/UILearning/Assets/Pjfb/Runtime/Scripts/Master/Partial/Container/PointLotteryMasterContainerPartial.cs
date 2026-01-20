
namespace Pjfb.Master {

    public partial class PointLotteryMasterContainer : MasterContainerBase<PointLotteryMasterObject> {
        long GetDefaultKey(PointLotteryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
