
namespace Pjfb.Master {

    public partial class BattleReserveFormationRoundMasterContainer : MasterContainerBase<BattleReserveFormationRoundMasterObject> {
        long GetDefaultKey(BattleReserveFormationRoundMasterObject masterObject){
            return masterObject.id;
        }
    }
}
