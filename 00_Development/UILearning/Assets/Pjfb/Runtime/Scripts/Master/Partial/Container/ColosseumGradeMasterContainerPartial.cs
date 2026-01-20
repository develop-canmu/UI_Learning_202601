
using System.Linq;

namespace Pjfb.Master {

    public partial class ColosseumGradeMasterContainer : MasterContainerBase<ColosseumGradeMasterObject> {
        long GetDefaultKey(ColosseumGradeMasterObject masterObject){
            return masterObject.id;
        }
        
        public ColosseumGradeMasterObject GetJoinedColosseumGradeMasterObject(long mColosseumGradeGroupId, long gradeNumber)
        {
            return values.Where(data => data.mColosseumGradeGroupId == mColosseumGradeGroupId)
                .FirstOrDefault(data => data.gradeNumber == gradeNumber);
        }
    }
}
