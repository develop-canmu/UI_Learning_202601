
namespace Pjfb.Master {

    public partial class TrainingBoardEventContentMasterContainer : MasterContainerBase<TrainingBoardEventContentMasterObject> {
        long GetDefaultKey(TrainingBoardEventContentMasterObject masterObject){
            return masterObject.id;
        }
    }
}
