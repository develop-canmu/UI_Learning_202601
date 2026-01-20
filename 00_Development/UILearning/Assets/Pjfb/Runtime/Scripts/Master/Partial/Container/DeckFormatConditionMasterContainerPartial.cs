
namespace Pjfb.Master {

    public partial class DeckFormatConditionMasterContainer : MasterContainerBase<DeckFormatConditionMasterObject> {
        long GetDefaultKey(DeckFormatConditionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
