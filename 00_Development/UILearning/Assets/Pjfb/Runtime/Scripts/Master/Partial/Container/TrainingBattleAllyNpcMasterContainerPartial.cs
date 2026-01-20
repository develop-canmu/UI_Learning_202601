
namespace Pjfb.Master {

    public partial class TrainingBattleAllyNpcMasterContainer : MasterContainerBase<TrainingBattleAllyNpcMasterObject> {
        long GetDefaultKey(TrainingBattleAllyNpcMasterObject masterObject){
            return masterObject.id;
        }
    }
}
