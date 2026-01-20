
namespace Pjfb.Master {

    public partial class LangDictionaryCnMasterContainer : MasterContainerBase<LangDictionaryCnMasterObject> {
        long GetDefaultKey(LangDictionaryCnMasterObject masterObject){
            return masterObject.id;
        }
    }
}
