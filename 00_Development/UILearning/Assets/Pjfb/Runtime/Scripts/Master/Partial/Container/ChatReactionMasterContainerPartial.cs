
namespace Pjfb.Master {

    public partial class ChatReactionMasterContainer : MasterContainerBase<ChatReactionMasterObject> {
        long GetDefaultKey(ChatReactionMasterObject masterObject){
            return masterObject.id;
        }
    }
}
