
namespace Pjfb.Master {

    public partial class BattleConquestFieldSpotMasterContainer : MasterContainerBase<BattleConquestFieldSpotMasterObject> {
        long GetDefaultKey(BattleConquestFieldSpotMasterObject masterObject){
            return masterObject.id;
        }
    }
}
