
namespace Pjfb.Master {

    public partial class CharaDeckTacticsAbilityMasterContainer : MasterContainerBase<CharaDeckTacticsAbilityMasterObject> {
        long GetDefaultKey(CharaDeckTacticsAbilityMasterObject masterObject){
            return masterObject.id;
        }
    }
}
