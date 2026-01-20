using System.Linq;

namespace Pjfb.Master {

    public partial class HuntStageMasterContainer
    {
        long GetDefaultKey(HuntStageMasterObject masterObject){
            return masterObject.id;
        }
        
        public HuntStageMasterObject[] FindStages(long mHuntId)
        {
            return values.Where(value => value.mHuntId == mHuntId).ToArray();
        }
    }
}
