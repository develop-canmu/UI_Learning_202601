
namespace Pjfb.Master {

    public partial class HuntEnemyScenarioMasterContainer : MasterContainerBase<HuntEnemyScenarioMasterObject> {
        long GetDefaultKey(HuntEnemyScenarioMasterObject masterObject){
            return masterObject.id;
        }
        
        public HuntEnemyScenarioMasterObject FindScenario(long mHuntEnemyId)
        {
            foreach(HuntEnemyScenarioMasterObject v in values)
            {
				if(v.mHuntEnemyId == mHuntEnemyId)return v;
            }
            return null;
        }
    }
}
