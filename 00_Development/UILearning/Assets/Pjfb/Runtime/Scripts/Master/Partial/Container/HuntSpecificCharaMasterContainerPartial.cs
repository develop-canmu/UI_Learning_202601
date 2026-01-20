
namespace Pjfb.Master {

    public partial class HuntSpecificCharaMasterContainer : MasterContainerBase<HuntSpecificCharaMasterObject> {
        long GetDefaultKey(HuntSpecificCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
