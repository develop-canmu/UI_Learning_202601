
namespace Pjfb.Master {

    public partial class TrainingCardMasterContainer : MasterContainerBase<TrainingCardMasterObject> {
        long GetDefaultKey(TrainingCardMasterObject masterObject){
            return masterObject.id;
        }
    }
}
