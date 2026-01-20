
using System.Linq;

namespace Pjfb.Master {

    public partial class FestivalItemMasterContainer : MasterContainerBase<FestivalItemMasterObject> {
        long GetDefaultKey(FestivalItemMasterObject masterObject){
            return masterObject.id;
        }
        
        public FestivalItemMasterObject FindDataByMFestivalTimetableId(long mFestivalTimetableId)
        {
            return values.FirstOrDefault(x => x.mFestivalTimetableId == mFestivalTimetableId);
        }
    }
}
