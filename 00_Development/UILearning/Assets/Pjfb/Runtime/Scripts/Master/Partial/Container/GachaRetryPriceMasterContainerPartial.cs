
namespace Pjfb.Master {

    public partial class GachaRetryPriceMasterContainer : MasterContainerBase<GachaRetryPriceMasterObject> {
        
        public GachaRetryPriceMasterObject FindDateByRetryId( long retryId ){
            foreach(var val in values){
                if( val.mGachaRetryId == retryId ) {
                    return val;
                }
            }
            return null;
        }

        public GachaRetryPriceMasterObject FindDateByRetryIdAndCount( long retryId, long count ){
            foreach(var val in values){
                if( val.mGachaRetryId == retryId && val.invokeCount == count ) {
                    return val;
                }
            }
            return null;
        }

        long GetDefaultKey(GachaRetryPriceMasterObject masterObject){
            return masterObject.id;
        }

        
    }
}
