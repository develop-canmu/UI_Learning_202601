
namespace Pjfb.Master {

    public partial class HuntDifficultyMasterContainer : MasterContainerBase<HuntDifficultyMasterObject> {
        long GetDefaultKey(HuntDifficultyMasterObject masterObject){
            return masterObject.id;
        }
    }
}
