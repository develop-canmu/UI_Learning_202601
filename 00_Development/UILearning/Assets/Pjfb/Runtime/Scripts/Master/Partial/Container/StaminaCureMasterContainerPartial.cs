
using System.Linq;

namespace Pjfb.Master {

    public partial class StaminaCureMasterContainer : MasterContainerBase<StaminaCureMasterObject> {
        long GetDefaultKey(StaminaCureMasterObject masterObject){
            return masterObject.id;
        }

        public StaminaCureMasterObject FindWithMPointId(int mPointId){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.FirstOrDefault(item => item.mPointId == mPointId);
        }
    }
}
