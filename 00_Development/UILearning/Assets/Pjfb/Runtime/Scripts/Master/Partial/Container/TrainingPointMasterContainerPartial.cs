
namespace Pjfb.Master {

    public partial class TrainingPointMasterContainer : MasterContainerBase<TrainingPointMasterObject> {
        long GetDefaultKey(TrainingPointMasterObject masterObject){
            return masterObject.id;
        }
    }
}
