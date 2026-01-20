
namespace Pjfb.Master {

    public partial class TrainingBoardMasterContainer : MasterContainerBase<TrainingBoardMasterObject> {
        long GetDefaultKey(TrainingBoardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
