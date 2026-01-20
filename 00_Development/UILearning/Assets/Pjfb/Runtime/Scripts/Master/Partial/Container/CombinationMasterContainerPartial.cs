
namespace Pjfb.Master {

    public partial class CombinationMasterContainer : MasterContainerBase<CombinationMasterObject> {
        long GetDefaultKey(CombinationMasterObject masterObject){
            return masterObject.id;
        }
    }
}
