
namespace Pjfb.Master {

    public partial class SkillCharaMasterContainer : MasterContainerBase<SkillCharaMasterObject> {
        long GetDefaultKey(SkillCharaMasterObject masterObject){
            return masterObject.id;
        }
    }
}
