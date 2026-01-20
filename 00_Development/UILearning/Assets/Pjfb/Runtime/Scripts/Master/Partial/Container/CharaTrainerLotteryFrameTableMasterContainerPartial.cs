
namespace Pjfb.Master {

    public partial class CharaTrainerLotteryFrameTableMasterContainer : MasterContainerBase<CharaTrainerLotteryFrameTableMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryFrameTableMasterObject masterObject){
            return masterObject.id;
        }
    }
}
