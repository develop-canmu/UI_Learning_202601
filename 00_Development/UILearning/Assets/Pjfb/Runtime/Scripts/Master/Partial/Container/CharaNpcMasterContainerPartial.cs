
namespace Pjfb.Master {

    public partial class CharaNpcMasterContainer : MasterContainerBase<CharaNpcMasterObject> {
        long GetDefaultKey(CharaNpcMasterObject masterObject){
            return masterObject.id;
        }
    }
}
