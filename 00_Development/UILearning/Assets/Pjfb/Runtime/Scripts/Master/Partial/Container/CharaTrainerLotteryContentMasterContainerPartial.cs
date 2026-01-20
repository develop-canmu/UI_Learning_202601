
namespace Pjfb.Master {

    public partial class CharaTrainerLotteryContentMasterContainer : MasterContainerBase<CharaTrainerLotteryContentMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryContentMasterObject masterObject){
            return masterObject.id;
        }
    }
}
