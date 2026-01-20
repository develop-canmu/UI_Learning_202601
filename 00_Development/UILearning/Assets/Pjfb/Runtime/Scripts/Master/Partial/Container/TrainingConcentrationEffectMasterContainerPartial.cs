
namespace Pjfb.Master {

    public partial class TrainingConcentrationEffectMasterContainer : MasterContainerBase<TrainingConcentrationEffectMasterObject> {
        long GetDefaultKey(TrainingConcentrationEffectMasterObject masterObject){
            return masterObject.id;
        }
    }
}
