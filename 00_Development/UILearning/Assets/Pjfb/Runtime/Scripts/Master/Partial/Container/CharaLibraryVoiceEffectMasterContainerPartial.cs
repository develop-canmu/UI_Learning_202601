
namespace Pjfb.Master {

    public partial class CharaLibraryVoiceEffectMasterContainer : MasterContainerBase<CharaLibraryVoiceEffectMasterObject> {
        long GetDefaultKey(CharaLibraryVoiceEffectMasterObject masterObject){
            return masterObject.id;
        }
    }
}
