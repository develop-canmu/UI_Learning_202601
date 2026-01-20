
using System.Linq;

namespace Pjfb.Master {

    public partial class PointGetBonusMasterContainer : MasterContainerBase<PointGetBonusMasterObject> {
        long GetDefaultKey(PointGetBonusMasterObject masterObject){
            return masterObject.id;
        }

        public PointGetBonusMasterObject GetObjectByAdminTagId(long _adminTagId)
        {
            return values.FirstOrDefault(data => data.adminTagId == _adminTagId);
        }
    }
}
