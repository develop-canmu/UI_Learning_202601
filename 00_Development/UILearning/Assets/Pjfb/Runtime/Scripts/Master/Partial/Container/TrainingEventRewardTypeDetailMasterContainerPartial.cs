
namespace Pjfb.Master {

    public partial class TrainingEventRewardTypeDetailMasterContainer : MasterContainerBase<TrainingEventRewardTypeDetailMasterObject> {
        long GetDefaultKey(TrainingEventRewardTypeDetailMasterObject masterObject){
            return masterObject.id;
        }
    }
}
