
namespace Pjfb.Master {

    public partial class TrainingCardConditionEffectMasterContainer : MasterContainerBase<TrainingCardConditionEffectMasterObject> {
        long GetDefaultKey(TrainingCardConditionEffectMasterObject masterObject){
            return masterObject.id;
        }
    }
}
