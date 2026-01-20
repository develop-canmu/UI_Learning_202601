
namespace Pjfb.Master {

    public partial class GuildRoleMasterContainer : MasterContainerBase<GuildRoleMasterObject> {
        long GetDefaultKey(GuildRoleMasterObject masterObject){
            return masterObject.id;
        }
    }
}
