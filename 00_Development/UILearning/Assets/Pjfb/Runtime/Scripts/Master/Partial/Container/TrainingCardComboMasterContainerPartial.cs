
namespace Pjfb.Master {

    public partial class TrainingCardComboMasterContainer : MasterContainerBase<TrainingCardComboMasterObject> {
        long GetDefaultKey(TrainingCardComboMasterObject masterObject){
            return masterObject.id;
        }
    }
}
