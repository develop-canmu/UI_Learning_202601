
namespace Pjfb.Master {

    public partial class TrainingConcentrationMasterContainer : MasterContainerBase<TrainingConcentrationMasterObject> {
        long GetDefaultKey(TrainingConcentrationMasterObject masterObject){
            return masterObject.id;
        }
    }
}
