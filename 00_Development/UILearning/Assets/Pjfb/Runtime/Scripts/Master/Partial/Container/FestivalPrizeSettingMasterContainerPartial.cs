
namespace Pjfb.Master {

    public partial class FestivalPrizeSettingMasterContainer : MasterContainerBase<FestivalPrizeSettingMasterObject> {
        long GetDefaultKey(FestivalPrizeSettingMasterObject masterObject){
            return masterObject.id;
        }
    }
}
