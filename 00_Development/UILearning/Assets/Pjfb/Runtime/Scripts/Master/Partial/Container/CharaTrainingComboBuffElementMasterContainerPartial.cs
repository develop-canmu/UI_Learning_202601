
namespace Pjfb.Master {

    public partial class CharaTrainingComboBuffElementMasterContainer : MasterContainerBase<CharaTrainingComboBuffElementMasterObject> {
        long GetDefaultKey(CharaTrainingComboBuffElementMasterObject masterObject){
            return masterObject.id;
        }
    }
}
