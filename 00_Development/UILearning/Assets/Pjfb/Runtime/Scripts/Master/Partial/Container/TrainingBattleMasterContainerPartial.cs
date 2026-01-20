
namespace Pjfb.Master {

    public partial class TrainingBattleMasterContainer : MasterContainerBase<TrainingBattleMasterObject> {
        long GetDefaultKey(TrainingBattleMasterObject masterObject){
            return masterObject.id;
        }
    }
}
