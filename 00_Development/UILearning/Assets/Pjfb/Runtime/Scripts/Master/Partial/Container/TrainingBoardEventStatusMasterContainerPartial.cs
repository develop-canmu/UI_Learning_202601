
namespace Pjfb.Master {

    public partial class TrainingBoardEventStatusMasterContainer : MasterContainerBase<TrainingBoardEventStatusMasterObject> {
        long GetDefaultKey(TrainingBoardEventStatusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
