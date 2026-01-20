
namespace Pjfb.Master {

    public partial class FestivalTimetableMasterContainer : MasterContainerBase<FestivalTimetableMasterObject> {
        long GetDefaultKey(FestivalTimetableMasterObject masterObject){
            return masterObject.id;
        }
        
        public FestivalTimetableMasterObject FindByFestivalId(long id)
        {
            foreach(FestivalTimetableMasterObject value in values)
            {
                if(value.mFestivalId == id)return value;
            }
            return null;
        }
        
    }
}
