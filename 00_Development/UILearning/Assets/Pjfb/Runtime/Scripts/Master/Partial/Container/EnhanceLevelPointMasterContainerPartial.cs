
namespace Pjfb.Master {

    public partial class EnhanceLevelPointMasterContainer : MasterContainerBase<EnhanceLevelPointMasterObject> {
        long GetDefaultKey(EnhanceLevelPointMasterObject masterObject){
            return masterObject.id;
        }
    }
}
