
namespace Pjfb.Master {

    public partial class RarityMasterContainer : MasterContainerBase<RarityMasterObject> {
        long GetDefaultKey(RarityMasterObject masterObject){
            return masterObject.id;
        }
    }
}
