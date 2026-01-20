
namespace Pjfb.Master {

    public partial class ProfileFrameMasterContainer : MasterContainerBase<ProfileFrameMasterObject> {
        long GetDefaultKey(ProfileFrameMasterObject masterObject){
            return masterObject.id;
        }
    }
}
