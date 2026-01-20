
namespace Pjfb.Master {

    public partial class StaminaMasterContainer : MasterContainerBase<StaminaMasterObject> {
        long GetDefaultKey(StaminaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
