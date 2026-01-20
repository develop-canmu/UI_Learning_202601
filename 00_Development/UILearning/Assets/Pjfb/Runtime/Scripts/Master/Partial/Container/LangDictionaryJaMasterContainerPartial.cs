
namespace Pjfb.Master {

    public partial class LangDictionaryJaMasterContainer : MasterContainerBase<LangDictionaryJaMasterObject> {
        long GetDefaultKey(LangDictionaryJaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
