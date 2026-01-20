
namespace Pjfb.Master {

    public partial class ColosseumBattleCorrectionMasterContainer : MasterContainerBase<ColosseumBattleCorrectionMasterObject> {
        long GetDefaultKey(ColosseumBattleCorrectionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
