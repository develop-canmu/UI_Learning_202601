
namespace Pjfb.Master {

    public partial class CharaComboAbilityMasterContainer : MasterContainerBase<CharaComboAbilityMasterObject> {
        long GetDefaultKey(CharaComboAbilityMasterObject masterObject){
            return masterObject.id;
        }
    }
}
