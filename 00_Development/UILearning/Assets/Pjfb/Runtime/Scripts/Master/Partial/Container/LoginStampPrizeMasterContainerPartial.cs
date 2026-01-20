
namespace Pjfb.Master {

    public partial class LoginStampPrizeMasterContainer : MasterContainerBase<LoginStampPrizeMasterObject> {
        long GetDefaultKey(LoginStampPrizeMasterObject masterObject){
            return masterObject.id;
        }


        public LoginStampPrizeMasterObject FindData(long prizeType, long prizeNumber){
            foreach( var val in values ) {
                if( val.prizeType == prizeType && val.prizeNumber == prizeNumber ) {
                    return val;
                }
            }
            CruFramework.Logger.LogError("not find data : prizeType = " + prizeType + ", prizeNumber = " + prizeNumber);
            return null;
        }
    }
}
