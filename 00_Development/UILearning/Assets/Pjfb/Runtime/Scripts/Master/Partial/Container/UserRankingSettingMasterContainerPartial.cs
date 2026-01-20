
namespace Pjfb.Master {

    public partial class UserRankingSettingMasterContainer : MasterContainerBase<UserRankingSettingMasterObject> {
        long GetDefaultKey(UserRankingSettingMasterObject masterObject){
            return masterObject.id;
        }
    }
}
