
namespace Pjfb.Master {

    public partial class BattleReserveFormationMasterContainer : MasterContainerBase<BattleReserveFormationMasterObject> {
        long GetDefaultKey(BattleReserveFormationMasterObject masterObject){
            return masterObject.id;
        }
    }
}
