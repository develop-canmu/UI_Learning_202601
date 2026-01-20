
namespace Pjfb.Master {

    public partial class TrainingScenarioStatusBonusMasterContainer : MasterContainerBase<TrainingScenarioStatusBonusMasterObject> {
        long GetDefaultKey(TrainingScenarioStatusBonusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
