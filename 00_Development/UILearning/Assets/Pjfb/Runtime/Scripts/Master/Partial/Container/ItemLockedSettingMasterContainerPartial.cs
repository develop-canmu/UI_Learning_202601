
namespace Pjfb.Master {

    public partial class ItemLockedSettingMasterContainer : MasterContainerBase<ItemLockedSettingMasterObject> {
        long GetDefaultKey(ItemLockedSettingMasterObject masterObject){
            return masterObject.id;
        }
    }
}
