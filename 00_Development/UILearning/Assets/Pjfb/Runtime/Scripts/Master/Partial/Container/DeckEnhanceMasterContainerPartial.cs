
namespace Pjfb.Master {

    public partial class DeckEnhanceMasterContainer : MasterContainerBase<DeckEnhanceMasterObject> {
        long GetDefaultKey(DeckEnhanceMasterObject masterObject){
            return masterObject.id;
        }
    }
}
