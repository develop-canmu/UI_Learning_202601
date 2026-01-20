
namespace Pjfb.Master {

    public partial class CharaTrainingComboBuffStatusMasterContainer : MasterContainerBase<CharaTrainingComboBuffStatusMasterObject> {
        long GetDefaultKey(CharaTrainingComboBuffStatusMasterObject masterObject){
            return masterObject.id;
        }
    }
}
