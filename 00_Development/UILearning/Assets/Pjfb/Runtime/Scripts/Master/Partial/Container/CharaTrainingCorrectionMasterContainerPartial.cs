
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class CharaTrainingCorrectionMasterContainer : MasterContainerBase<CharaTrainingCorrectionMasterObject> {
        long GetDefaultKey(CharaTrainingCorrectionMasterObject masterObject){
            return masterObject.id;
        }
        

        // MCharIdから取得
        public CharaTrainingCorrectionMasterObject FindByMCharId(long mCharId, long level)
        {
        
            CharaTrainingCorrectionMasterObject result = null;
        
            foreach(CharaTrainingCorrectionMasterObject value in values)
            {
                if(value.mCharaId == mCharId && value.minLevel <= level)
                {
                    if(result == null || result.minLevel < value.minLevel)
                    {
                        result = value;
                    }
                }
            }
            
            return result;            
        }
    }
}
