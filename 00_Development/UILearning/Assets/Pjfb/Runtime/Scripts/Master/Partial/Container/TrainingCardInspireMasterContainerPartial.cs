
namespace Pjfb.Master {

    public partial class TrainingCardInspireMasterContainer : MasterContainerBase<TrainingCardInspireMasterObject> {
        long GetDefaultKey(TrainingCardInspireMasterObject masterObject){
            return masterObject.id;
        }
    }
}
