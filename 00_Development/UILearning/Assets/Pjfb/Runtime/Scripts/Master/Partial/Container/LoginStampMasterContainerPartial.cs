
namespace Pjfb.Master {

    public partial class LoginStampMasterContainer : MasterContainerBase<LoginStampMasterObject> {
        long GetDefaultKey(LoginStampMasterObject masterObject){
            return masterObject.id;
        }
    }
}
