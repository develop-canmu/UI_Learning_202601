
namespace Pjfb.Master {

    public partial class StaminaUseToMasterContainer : MasterContainerBase<StaminaUseToMasterObject> {
        long GetDefaultKey(StaminaUseToMasterObject masterObject){
            return masterObject.id;
        }
    }
}
