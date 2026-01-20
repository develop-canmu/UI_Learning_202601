
namespace Pjfb.Master {

    public partial class CombinationCharaMasterContainer : MasterContainerBase<CombinationCharaMasterObject> {
        long GetDefaultKey(CombinationCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
