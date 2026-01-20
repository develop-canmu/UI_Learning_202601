
namespace Pjfb.Master {

    public partial class LoginPassMasterContainer : MasterContainerBase<LoginPassMasterObject> {
        long GetDefaultKey(LoginPassMasterObject masterObject){
            return masterObject.id;
        }
        
        public LoginPassMasterObject FindDataByMBillingRewardBonusId(long mBillingRewardBonusId)
        {
            LoginPassMasterObject result = null;
            foreach(LoginPassMasterObject value in values)
            {
                // 課金パックIdが違う
                if(value.mBillingRewardBonusId != mBillingRewardBonusId)continue;
                result = value;
            }
            
            return result;
        }
    }
}
