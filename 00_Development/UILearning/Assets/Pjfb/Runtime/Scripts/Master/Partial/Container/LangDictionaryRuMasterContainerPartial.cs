
namespace Pjfb.Master {

    public partial class LangDictionaryRuMasterContainer : MasterContainerBase<LangDictionaryRuMasterObject> {
        long GetDefaultKey(LangDictionaryRuMasterObject masterObject){
            return masterObject.id;
        }
    }
}
