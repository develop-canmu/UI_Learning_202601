
namespace Pjfb.Master {

    public partial class TrainingIntentionalEventMasterContainer : MasterContainerBase<TrainingIntentionalEventMasterObject> {
        long GetDefaultKey(TrainingIntentionalEventMasterObject masterObject){
            return masterObject.id;
        }
    }
}
