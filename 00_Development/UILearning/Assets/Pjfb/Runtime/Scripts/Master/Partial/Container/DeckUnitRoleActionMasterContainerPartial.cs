
namespace Pjfb.Master {

    public partial class DeckUnitRoleActionMasterContainer : MasterContainerBase<DeckUnitRoleActionMasterObject> {
        long GetDefaultKey(DeckUnitRoleActionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
