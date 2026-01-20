
namespace Pjfb.Master {

    public partial class DeckTacticsMasterContainer : MasterContainerBase<DeckTacticsMasterObject> {
        long GetDefaultKey(DeckTacticsMasterObject masterObject){
            return masterObject.id;
        }
    }
}
