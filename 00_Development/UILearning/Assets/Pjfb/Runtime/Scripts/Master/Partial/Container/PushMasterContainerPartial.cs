
namespace Pjfb.Master {

    public partial class PushMasterContainer : MasterContainerBase<PushMasterObject> {
        long GetDefaultKey(PushMasterObject masterObject){
            return masterObject.id;
        }
    }
}
