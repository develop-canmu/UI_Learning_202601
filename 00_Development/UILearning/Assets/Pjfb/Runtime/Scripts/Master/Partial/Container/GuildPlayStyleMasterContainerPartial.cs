
namespace Pjfb.Master {

    public partial class GuildPlayStyleMasterContainer : MasterContainerBase<GuildPlayStyleMasterObject> {
        long GetDefaultKey(GuildPlayStyleMasterObject masterObject){
            return masterObject.id;
        }
    }
}
