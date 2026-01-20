
namespace Pjfb.Master {

    public partial class TrainingBattleCorrectionMasterContainer : MasterContainerBase<TrainingBattleCorrectionMasterObject> {
        long GetDefaultKey(TrainingBattleCorrectionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
