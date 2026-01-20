
namespace Pjfb.Master {

    public partial class DailyMissionCategoryMasterContainer : MasterContainerBase<DailyMissionCategoryMasterObject> {
        long GetDefaultKey(DailyMissionCategoryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
