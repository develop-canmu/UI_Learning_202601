
namespace Pjfb.Master {

    public partial class TrainingPointStatusEffectMasterContainer : MasterContainerBase<TrainingPointStatusEffectMasterObject> {
        long GetDefaultKey(TrainingPointStatusEffectMasterObject masterObject){
            return masterObject.id;
        }
    }
}
