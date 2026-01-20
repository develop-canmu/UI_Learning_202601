
namespace Pjfb.Master {

    public partial class AbilityMasterContainer : MasterContainerBase<AbilityMasterObject> {
        long GetDefaultKey(AbilityMasterObject masterObject){
            return masterObject.id;
        }
    }
}
