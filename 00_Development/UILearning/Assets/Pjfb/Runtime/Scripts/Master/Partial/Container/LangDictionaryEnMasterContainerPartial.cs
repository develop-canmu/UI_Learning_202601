
namespace Pjfb.Master {

    public partial class LangDictionaryEnMasterContainer : MasterContainerBase<LangDictionaryEnMasterObject> {
        long GetDefaultKey(LangDictionaryEnMasterObject masterObject){
            return masterObject.id;
        }
    }
}
