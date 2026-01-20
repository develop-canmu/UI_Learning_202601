
namespace Pjfb.Master {

    public partial class DeckUnitRoleMasterContainer : MasterContainerBase<DeckUnitRoleMasterObject> {
        long GetDefaultKey(DeckUnitRoleMasterObject masterObject){
            return masterObject.id;
        }
    }
}
