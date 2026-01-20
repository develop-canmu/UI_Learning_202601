
namespace Pjfb.Master {

    public partial class GachaRetryMasterContainer : MasterContainerBase<GachaRetryMasterObject> {
        long GetDefaultKey(GachaRetryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
