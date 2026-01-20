
namespace Pjfb.Master {

    public partial class TrainingScenarioMasterContainer : MasterContainerBase<TrainingScenarioMasterObject> {
        long GetDefaultKey(TrainingScenarioMasterObject masterObject){
            return masterObject.id;
        }
    }
}
