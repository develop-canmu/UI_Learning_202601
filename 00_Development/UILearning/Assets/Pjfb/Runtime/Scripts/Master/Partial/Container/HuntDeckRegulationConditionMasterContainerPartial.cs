
namespace Pjfb.Master {

    public partial class HuntDeckRegulationConditionMasterContainer : MasterContainerBase<HuntDeckRegulationConditionMasterObject> {
        long GetDefaultKey(HuntDeckRegulationConditionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
