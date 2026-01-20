
namespace Pjfb.Master {

    public partial class CharaRarityChangeMasterContainer : MasterContainerBase<CharaRarityChangeMasterObject> {
        long GetDefaultKey(CharaRarityChangeMasterObject masterObject){
            return masterObject.id;
        }
    }
}
