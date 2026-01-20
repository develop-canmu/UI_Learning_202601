
namespace Pjfb.Master {

    public partial class TrainingBoardEventCharaMasterContainer : MasterContainerBase<TrainingBoardEventCharaMasterObject> {
        long GetDefaultKey(TrainingBoardEventCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
