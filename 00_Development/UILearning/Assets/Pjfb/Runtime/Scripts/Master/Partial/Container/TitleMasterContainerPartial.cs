
namespace Pjfb.Master {

    public partial class TitleMasterContainer : MasterContainerBase<TitleMasterObject> {
        long GetDefaultKey(TitleMasterObject masterObject){
            return masterObject.id;
        }
    }
}
