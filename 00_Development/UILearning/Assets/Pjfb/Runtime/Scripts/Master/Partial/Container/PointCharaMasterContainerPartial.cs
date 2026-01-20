
namespace Pjfb.Master {

    public partial class PointCharaMasterContainer : MasterContainerBase<PointCharaMasterObject> {
        long GetDefaultKey(PointCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
