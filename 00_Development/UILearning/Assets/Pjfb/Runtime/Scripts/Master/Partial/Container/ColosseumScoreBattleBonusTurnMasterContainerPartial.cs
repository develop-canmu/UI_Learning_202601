
namespace Pjfb.Master {

    public partial class ColosseumScoreBattleBonusTurnMasterContainer : MasterContainerBase<ColosseumScoreBattleBonusTurnMasterObject> {
        long GetDefaultKey(ColosseumScoreBattleBonusTurnMasterObject masterObject){
            return masterObject.id;
        }
    }
}
