
namespace Pjfb.Master {

    public partial class CharaTrainingCoachRankRewardMasterContainer : MasterContainerBase<CharaTrainingCoachRankRewardMasterObject> {
        long GetDefaultKey(CharaTrainingCoachRankRewardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
