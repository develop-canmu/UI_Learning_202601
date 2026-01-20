
namespace Pjfb.Master {

    public partial class CharaTrainerLotteryStatusMasterContainer : MasterContainerBase<CharaTrainerLotteryStatusMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryStatusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
