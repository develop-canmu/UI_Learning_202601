
namespace Pjfb.Master {

    public partial class LevelRewardPrizeMasterContainer : MasterContainerBase<LevelRewardPrizeMasterObject> {
        long GetDefaultKey(LevelRewardPrizeMasterObject masterObject){
            return masterObject.id;
        }
    }
}
