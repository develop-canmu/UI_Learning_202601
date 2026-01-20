
namespace Pjfb.Master {
    public enum EmblemType {
        General = 1,
        Special = 2
    }

    public partial class GuildEmblemMasterContainer : MasterContainerBase<GuildEmblemMasterObject> {
        long GetDefaultKey(GuildEmblemMasterObject masterObject){
            return masterObject.id;
        }
    }
}
