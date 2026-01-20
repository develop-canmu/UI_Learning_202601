
namespace Pjfb.Master {

    public partial class LangDictionaryItMasterContainer : MasterContainerBase<LangDictionaryItMasterObject> {
        long GetDefaultKey(LangDictionaryItMasterObject masterObject){
            return masterObject.id;
        }
    }
}
