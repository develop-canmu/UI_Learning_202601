
namespace Pjfb.Master {

    public partial class HuntDeckRegulationMasterContainer : MasterContainerBase<HuntDeckRegulationMasterObject> {
        long GetDefaultKey(HuntDeckRegulationMasterObject masterObject){
            return masterObject.id;
        }
    }
}
