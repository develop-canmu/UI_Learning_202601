
namespace Pjfb.Master {

    public partial class TrainingAutoCostMasterContainer : MasterContainerBase<TrainingAutoCostMasterObject> {
        long GetDefaultKey(TrainingAutoCostMasterObject masterObject){
            return masterObject.id;
        }
    }
}
