
namespace Pjfb.Master {

    public partial class ColosseumNpcGuildMasterContainer : MasterContainerBase<ColosseumNpcGuildMasterObject> {
        long GetDefaultKey(ColosseumNpcGuildMasterObject masterObject){
            return masterObject.id;
        }
    }
}
