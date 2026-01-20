
namespace Pjfb.Master {

    public partial class LangMasterContainer : MasterContainerBase<LangMasterObject> {
        long GetDefaultKey(LangMasterObject masterObject){
            return masterObject.id;
        }
    }
}
