
namespace Pjfb.Master {

    public partial class LangDictionaryPtMasterContainer : MasterContainerBase<LangDictionaryPtMasterObject> {
        long GetDefaultKey(LangDictionaryPtMasterObject masterObject){
            return masterObject.id;
        }
    }
}
