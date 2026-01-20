
namespace Pjfb.Master {

    public partial class DeckTacticsDetailMasterContainer : MasterContainerBase<DeckTacticsDetailMasterObject> {
        long GetDefaultKey(DeckTacticsDetailMasterObject masterObject){
            return masterObject.id;
        }
    }
}
