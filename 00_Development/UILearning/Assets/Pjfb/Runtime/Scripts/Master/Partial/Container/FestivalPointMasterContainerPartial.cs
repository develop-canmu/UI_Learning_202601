
namespace Pjfb.Master {

    public partial class FestivalPointMasterContainer : MasterContainerBase<FestivalPointMasterObject> {
        long GetDefaultKey(FestivalPointMasterObject masterObject){
            return masterObject.id;
        }
    }
}
