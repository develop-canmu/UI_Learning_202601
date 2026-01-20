
namespace Pjfb.Master {

    public partial class CharaTrainerLotteryMasterContainer : MasterContainerBase<CharaTrainerLotteryMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
