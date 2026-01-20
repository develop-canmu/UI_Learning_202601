
namespace Pjfb.Master {

    public partial class DeckEnhanceTrainingStatusMasterContainer : MasterContainerBase<DeckEnhanceTrainingStatusMasterObject> {
        long GetDefaultKey(DeckEnhanceTrainingStatusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
