
namespace Pjfb.Master {

    public partial class CharaComboAbilityElementMasterContainer : MasterContainerBase<CharaComboAbilityElementMasterObject> {
        long GetDefaultKey(CharaComboAbilityElementMasterObject masterObject){
            return masterObject.id;
        }
    }
}
