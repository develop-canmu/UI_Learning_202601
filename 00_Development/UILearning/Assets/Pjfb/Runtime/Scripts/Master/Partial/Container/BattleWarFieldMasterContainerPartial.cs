
namespace Pjfb.Master {

    public partial class BattleWarFieldMasterContainer : MasterContainerBase<BattleWarFieldMasterObject> {
        long GetDefaultKey(BattleWarFieldMasterObject masterObject){
            return masterObject.id;
        }
    }
}
