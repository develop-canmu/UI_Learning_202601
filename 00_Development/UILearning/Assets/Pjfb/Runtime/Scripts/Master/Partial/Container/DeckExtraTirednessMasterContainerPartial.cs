
namespace Pjfb.Master {

    public partial class DeckExtraTirednessMasterContainer : MasterContainerBase<DeckExtraTirednessMasterObject> {
        long GetDefaultKey(DeckExtraTirednessMasterObject masterObject){
            return masterObject.id;
        }
    }
}
