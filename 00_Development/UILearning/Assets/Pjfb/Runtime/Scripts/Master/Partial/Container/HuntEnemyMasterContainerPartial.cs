
using System.Linq;
using Pjfb.Rivalry;

namespace Pjfb.Master {

    public partial class HuntEnemyMasterContainer : MasterContainerBase<HuntEnemyMasterObject> {
        long GetDefaultKey(HuntEnemyMasterObject masterObject){
            return masterObject.id;
        }
        
        public HuntEnemyMasterObject[] FindByHuntId(int mHuntId)
        {
            return values.Where(value => value.mHuntId == mHuntId).ToArray();
        }

        public HuntEnemyMasterObject FindNextStageHunt(HuntEnemyMasterObject huntEnemyMasterObject)
        {
            var huntEnemyObjectList = RivalryManager.rivalryCacheData.huntEnemyObjectList.TryGetValue(huntEnemyMasterObject.mHuntId, out var huntEnemyObject) ? huntEnemyObject : null;
            var nextIndex = huntEnemyObjectList.FindIndex(data => data.id == huntEnemyMasterObject.id) + 1;
            if (nextIndex < 0 || nextIndex >= huntEnemyObjectList.Count) return null;
            return huntEnemyObjectList[nextIndex];
        }
    }
}
