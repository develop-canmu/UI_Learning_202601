
namespace Pjfb.Master {

    public partial class ColosseumEventGroupLabelMasterContainer : MasterContainerBase<ColosseumEventGroupLabelMasterObject> {
        long GetDefaultKey(ColosseumEventGroupLabelMasterObject masterObject){
            return masterObject.id;
        }
    }
}
