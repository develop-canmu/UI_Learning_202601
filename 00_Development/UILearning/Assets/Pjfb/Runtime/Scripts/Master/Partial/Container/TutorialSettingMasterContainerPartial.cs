
namespace Pjfb.Master {

    public partial class TutorialSettingMasterContainer : MasterContainerBase<TutorialSettingMasterObject> {
        long GetDefaultKey(TutorialSettingMasterObject masterObject){
            return masterObject.id;
        }
    }
}
