
namespace Pjfb.Master {

    public partial class PointExpiryMasterContainer : MasterContainerBase<PointExpiryMasterObject> {
        long GetDefaultKey(PointExpiryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
