
namespace Pjfb.Master {

    public partial class CharaLibraryProfileMasterContainer : MasterContainerBase<CharaLibraryProfileMasterObject> {
        long GetDefaultKey(CharaLibraryProfileMasterObject masterObject){
            return masterObject.id;
        }
    }
}
