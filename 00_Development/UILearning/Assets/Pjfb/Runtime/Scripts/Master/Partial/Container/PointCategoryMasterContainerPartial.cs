
namespace Pjfb.Master {

    public partial class PointCategoryMasterContainer : MasterContainerBase<PointCategoryMasterObject> {
        long GetDefaultKey(PointCategoryMasterObject masterObject){
            return masterObject.id;
        }
    }
}
