
namespace Pjfb.Master {

    public partial class FestivalMasterContainer : MasterContainerBase<FestivalMasterObject> {
        long GetDefaultKey(FestivalMasterObject masterObject){
            return masterObject.id;
        }
    }
}
