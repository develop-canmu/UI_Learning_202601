
namespace Pjfb.Master {

    public partial class TrainingBoardEventForkGroupMasterContainer : MasterContainerBase<TrainingBoardEventForkGroupMasterObject> {
        long GetDefaultKey(TrainingBoardEventForkGroupMasterObject masterObject){
            return masterObject.id;
        }
    }
}
