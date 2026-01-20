
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class EnhanceLevelMasterContainer : MasterContainerBase<EnhanceLevelMasterObject> {
        long GetDefaultKey(EnhanceLevelMasterObject masterObject){
            return masterObject.id;
        }
        
        private Dictionary<long, EnhanceLevelMasterObject[]> cacheFindByMEnhanceId = new Dictionary<long, EnhanceLevelMasterObject[]>();


        public EnhanceLevelMasterObject[] FindByMEnhanceId(long mEnhanceId)
        {
            // キャッシュから
            if(cacheFindByMEnhanceId.TryGetValue(mEnhanceId, out EnhanceLevelMasterObject[] r))
            {
                return r;
            }
			
            List<EnhanceLevelMasterObject> result = new List<EnhanceLevelMasterObject>();
            // データを探す
            foreach(EnhanceLevelMasterObject value in values)
            {
				if(value.mEnhanceId == mEnhanceId)
				{
                    result.Add(value);
                }
            }
            
            // キャッシュに入れる
            EnhanceLevelMasterObject[] arrayResult = result.ToArray();
            cacheFindByMEnhanceId.Add(mEnhanceId, arrayResult);
            return arrayResult;
        }
        
    }
}
