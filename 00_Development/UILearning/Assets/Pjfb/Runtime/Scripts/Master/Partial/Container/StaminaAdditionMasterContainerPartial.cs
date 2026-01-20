
using System.Linq;

namespace Pjfb.Master {

    public partial class StaminaAdditionMasterContainer : MasterContainerBase<StaminaAdditionMasterObject> {
        long GetDefaultKey(StaminaAdditionMasterObject masterObject){
            return masterObject.id;
        }

        public StaminaAdditionMasterObject FindDataByTagAndStaminaId(long tag, long mStaminaId)
        {
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.FirstOrDefault(item => item.adminTagId == tag && item.mStaminaId == mStaminaId);
        }
    }
}
