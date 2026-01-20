
namespace Pjfb.Master {

    public partial class DeckUnitRoleOperationMasterContainer : MasterContainerBase<DeckUnitRoleOperationMasterObject> {
        long GetDefaultKey(DeckUnitRoleOperationMasterObject masterObject){
            return masterObject.id;
        }
    }
}
