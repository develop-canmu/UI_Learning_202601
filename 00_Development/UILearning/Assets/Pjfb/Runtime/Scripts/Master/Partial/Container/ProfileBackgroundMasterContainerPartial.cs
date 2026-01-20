
namespace Pjfb.Master {

    public partial class ProfileBackgroundMasterContainer : MasterContainerBase<ProfileBackgroundMasterObject> {
        long GetDefaultKey(ProfileBackgroundMasterObject masterObject){
            return masterObject.id;
        }
    }
}
