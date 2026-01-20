
namespace Pjfb.Master {

    public partial class LangDictionaryFrMasterContainer : MasterContainerBase<LangDictionaryFrMasterObject> {
        long GetDefaultKey(LangDictionaryFrMasterObject masterObject){
            return masterObject.id;
        }
    }
}
