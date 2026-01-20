
namespace Pjfb.Master {

    public partial class LangDictionaryTwMasterContainer : MasterContainerBase<LangDictionaryTwMasterObject> {
        long GetDefaultKey(LangDictionaryTwMasterObject masterObject){
            return masterObject.id;
        }
    }
}
