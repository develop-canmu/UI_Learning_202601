
namespace Pjfb.Master {

    public partial class CharaTrainerLotterySlotMasterContainer : MasterContainerBase<CharaTrainerLotterySlotMasterObject> {
        long GetDefaultKey(CharaTrainerLotterySlotMasterObject masterObject){
            return masterObject.id;
        }
    }
}
