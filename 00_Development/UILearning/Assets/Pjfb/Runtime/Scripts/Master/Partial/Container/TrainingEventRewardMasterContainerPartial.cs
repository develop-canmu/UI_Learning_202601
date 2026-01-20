
namespace Pjfb.Master {

    public partial class TrainingEventRewardMasterContainer : MasterContainerBase<TrainingEventRewardMasterObject> {
        long GetDefaultKey(TrainingEventRewardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
