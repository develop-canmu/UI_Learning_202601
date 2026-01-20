
namespace Pjfb.Master {

    public partial class BattleGameliftMasterContainer : MasterContainerBase<BattleGameliftMasterObject> {
        long GetDefaultKey(BattleGameliftMasterObject masterObject){
            return masterObject.id;
        }
    }
}
