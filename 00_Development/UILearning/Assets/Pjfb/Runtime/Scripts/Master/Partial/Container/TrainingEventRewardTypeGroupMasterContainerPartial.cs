
namespace Pjfb.Master {

    public partial class TrainingEventRewardTypeGroupMasterContainer : MasterContainerBase<TrainingEventRewardTypeGroupMasterObject> {
        long GetDefaultKey(TrainingEventRewardTypeGroupMasterObject masterObject){
            return masterObject.id;
        }
    }
}
