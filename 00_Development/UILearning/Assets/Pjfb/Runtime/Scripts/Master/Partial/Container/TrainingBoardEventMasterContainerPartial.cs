
namespace Pjfb.Master {

    public partial class TrainingBoardEventMasterContainer : MasterContainerBase<TrainingBoardEventMasterObject> {
        long GetDefaultKey(TrainingBoardEventMasterObject masterObject){
            return masterObject.id;
        }
    }
}
