
namespace Pjfb.Master {

    public partial class ColosseumNpcMasterContainer : MasterContainerBase<ColosseumNpcMasterObject> {
        long GetDefaultKey(ColosseumNpcMasterObject masterObject){
            return masterObject.id;
        }
    }
}
