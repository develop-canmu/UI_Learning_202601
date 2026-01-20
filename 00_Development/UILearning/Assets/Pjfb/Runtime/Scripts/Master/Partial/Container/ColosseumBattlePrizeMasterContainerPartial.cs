
namespace Pjfb.Master {

    public partial class ColosseumBattlePrizeMasterContainer : MasterContainerBase<ColosseumBattlePrizeMasterObject> {
        long GetDefaultKey(ColosseumBattlePrizeMasterObject masterObject){
            return masterObject.id;
        }
    }
}
