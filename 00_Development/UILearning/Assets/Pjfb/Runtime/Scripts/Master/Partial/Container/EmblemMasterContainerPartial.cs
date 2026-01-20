
namespace Pjfb.Master {

    public partial class EmblemMasterContainer : MasterContainerBase<EmblemMasterObject> {
        long GetDefaultKey(EmblemMasterObject masterObject){
            return masterObject.id;
        }
    }
}
