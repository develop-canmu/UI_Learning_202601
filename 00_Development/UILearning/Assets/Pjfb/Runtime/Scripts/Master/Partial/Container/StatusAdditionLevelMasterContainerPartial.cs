
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class StatusAdditionLevelMasterContainer : MasterContainerBase<StatusAdditionLevelMasterObject> {
        long GetDefaultKey(StatusAdditionLevelMasterObject masterObject){
            return masterObject.id;
        }
        
        public StatusAdditionLevelMasterObject FindData(long id, long level)
        {
            foreach(StatusAdditionLevelMasterObject value in values)
            {
                if(value.mStatusAdditionId == id && value.level == level)return value;
            }
            
            return null;
        }
    }
}
