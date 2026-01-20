
namespace Pjfb.Master {

    public partial class BattleConquestFieldMasterContainer : MasterContainerBase<BattleConquestFieldMasterObject> {
        long GetDefaultKey(BattleConquestFieldMasterObject masterObject){
            return masterObject.id;
        }
    }
}
