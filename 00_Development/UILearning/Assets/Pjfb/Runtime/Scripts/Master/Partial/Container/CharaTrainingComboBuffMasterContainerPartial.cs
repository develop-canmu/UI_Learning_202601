
namespace Pjfb.Master {

    public partial class CharaTrainingComboBuffMasterContainer : MasterContainerBase<CharaTrainingComboBuffMasterObject> {
        long GetDefaultKey(CharaTrainingComboBuffMasterObject masterObject){
            return masterObject.id;
        }
    }
}
