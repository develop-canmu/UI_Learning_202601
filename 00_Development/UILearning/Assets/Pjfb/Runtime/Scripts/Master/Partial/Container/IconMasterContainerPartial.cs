
namespace Pjfb.Master {

    public partial class IconMasterContainer : MasterContainerBase<IconMasterObject> {
        long GetDefaultKey(IconMasterObject masterObject){
            return masterObject.id;
        }
    }
}
