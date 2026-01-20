
namespace Pjfb.Master {

    public partial class TrainingUnitMasterContainer : MasterContainerBase<TrainingUnitMasterObject> {
        long GetDefaultKey(TrainingUnitMasterObject masterObject){
            return masterObject.id;
        }
    }
}
