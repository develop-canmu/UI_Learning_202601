
namespace Pjfb.Master {

    public partial class FestivalPrizeContentMasterContainer : MasterContainerBase<FestivalPrizeContentMasterObject> {
        long GetDefaultKey(FestivalPrizeContentMasterObject masterObject){
            return masterObject.id;
        }
    }
}
