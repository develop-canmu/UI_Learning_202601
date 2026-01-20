
namespace Pjfb.Master {

    public partial class PlayerEnhanceMasterContainer : MasterContainerBase<PlayerEnhanceMasterObject> {
        long GetDefaultKey(PlayerEnhanceMasterObject masterObject){
            return masterObject.id;
        }
    }
}
