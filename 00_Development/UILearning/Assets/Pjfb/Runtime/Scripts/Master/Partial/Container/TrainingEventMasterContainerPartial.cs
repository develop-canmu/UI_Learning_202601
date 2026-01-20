using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public partial class TrainingEventMasterContainer : MasterContainerBase<TrainingEventMasterObject> {
        long GetDefaultKey(TrainingEventMasterObject masterObject){
            return masterObject.id;
        }

        //// <summary> EventGroupが一致するデータを検索 </summary>
        public IEnumerable<TrainingEventMasterObject> FindEventGroup(long eventGroup)
        {
           return values.Where(x => x.eventGroup == eventGroup).OrderBy(x => x.id);
        }
    }
}
