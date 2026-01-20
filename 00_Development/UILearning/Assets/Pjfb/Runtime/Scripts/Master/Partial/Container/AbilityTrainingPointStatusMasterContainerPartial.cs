
namespace Pjfb.Master {

    public partial class AbilityTrainingPointStatusMasterContainer : MasterContainerBase<AbilityTrainingPointStatusMasterObject> {
        long GetDefaultKey(AbilityTrainingPointStatusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
