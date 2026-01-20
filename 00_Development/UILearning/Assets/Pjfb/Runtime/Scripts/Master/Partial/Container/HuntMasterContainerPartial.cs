
namespace Pjfb.Master {

    public partial class HuntMasterContainer : MasterContainerBase<HuntMasterObject> {
        long GetDefaultKey(HuntMasterObject masterObject){
            return masterObject.id;
        }
    }
}
