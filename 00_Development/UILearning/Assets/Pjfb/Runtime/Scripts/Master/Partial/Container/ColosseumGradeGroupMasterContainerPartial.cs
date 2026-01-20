
namespace Pjfb.Master {

    public partial class ColosseumGradeGroupMasterContainer : MasterContainerBase<ColosseumGradeGroupMasterObject> {
        long GetDefaultKey(ColosseumGradeGroupMasterObject masterObject){
            return masterObject.id;
        }
    }
}
