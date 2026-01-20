
namespace Pjfb.Master {

    public partial class LangDictionaryKoMasterContainer : MasterContainerBase<LangDictionaryKoMasterObject> {
        long GetDefaultKey(LangDictionaryKoMasterObject masterObject){
            return masterObject.id;
        }
    }
}
