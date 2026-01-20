
namespace Pjfb.Master {

    public partial class ProfilePartMasterContainer : MasterContainerBase<ProfilePartMasterObject> {
        long GetDefaultKey(ProfilePartMasterObject masterObject){
            return masterObject.id;
        }
    }
}
