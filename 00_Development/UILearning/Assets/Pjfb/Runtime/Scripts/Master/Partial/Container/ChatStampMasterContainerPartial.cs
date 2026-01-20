
namespace Pjfb.Master {

    public partial class ChatStampMasterContainer : MasterContainerBase<ChatStampMasterObject> {
        long GetDefaultKey(ChatStampMasterObject masterObject){
            return masterObject.id;
        }
    }
}
