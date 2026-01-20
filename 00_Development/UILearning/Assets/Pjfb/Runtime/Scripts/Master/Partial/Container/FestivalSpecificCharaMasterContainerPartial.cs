
namespace Pjfb.Master {

    public partial class FestivalSpecificCharaMasterContainer : MasterContainerBase<FestivalSpecificCharaMasterObject> {
        long GetDefaultKey(FestivalSpecificCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
