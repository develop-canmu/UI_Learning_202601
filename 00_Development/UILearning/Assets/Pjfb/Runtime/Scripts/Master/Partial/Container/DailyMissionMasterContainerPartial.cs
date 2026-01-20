
namespace Pjfb.Master {

    public partial class DailyMissionMasterContainer : MasterContainerBase<DailyMissionMasterObject> {
        long GetDefaultKey(DailyMissionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
