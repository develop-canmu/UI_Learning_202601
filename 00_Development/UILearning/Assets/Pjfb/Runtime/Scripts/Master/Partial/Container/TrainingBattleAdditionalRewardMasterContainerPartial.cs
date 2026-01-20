
namespace Pjfb.Master {

    public partial class TrainingBattleAdditionalRewardMasterContainer : MasterContainerBase<TrainingBattleAdditionalRewardMasterObject> {
        long GetDefaultKey(TrainingBattleAdditionalRewardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
