
namespace Pjfb.Master {

    public partial class TrainingCoachRankMasterContainer : MasterContainerBase<TrainingCoachRankMasterObject> {
        long GetDefaultKey(TrainingCoachRankMasterObject masterObject){
            return masterObject.id;
        }
    }
}
