
namespace Pjfb.Master {

    public partial class CharaRankPointMasterContainer : MasterContainerBase<CharaRankPointMasterObject> {
        long GetDefaultKey(CharaRankPointMasterObject masterObject){
            return masterObject.id;
        }
    }
}
