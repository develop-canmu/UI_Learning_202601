
namespace Pjfb.Master {

    public partial class LangDictionaryEsMasterContainer : MasterContainerBase<LangDictionaryEsMasterObject> {
        long GetDefaultKey(LangDictionaryEsMasterObject masterObject){
            return masterObject.id;
        }
    }
}
