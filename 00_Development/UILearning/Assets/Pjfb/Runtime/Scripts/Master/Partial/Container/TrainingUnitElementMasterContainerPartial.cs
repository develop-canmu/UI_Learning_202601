
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class TrainingUnitElementMasterContainer : MasterContainerBase<TrainingUnitElementMasterObject> {
        long GetDefaultKey(TrainingUnitElementMasterObject masterObject){
            return masterObject.id;
        }
        
        private Dictionary<long, TrainingUnitElementMasterObject[]> unitIdCache = new Dictionary<long, TrainingUnitElementMasterObject[]>();

        public TrainingUnitElementMasterObject[] FindDataByUnitId(long unitId)
        {
            // キャッシュから返す
            if(unitIdCache.TryGetValue(unitId, out TrainingUnitElementMasterObject[] r))
            {
                return r;
            }
            
            List<TrainingUnitElementMasterObject> list = new List<TrainingUnitElementMasterObject>();

            foreach(TrainingUnitElementMasterObject value in values)
            {
                if(value.mTrainingUnitId == unitId)
                {
                    list.Add(value);
                }
            }
            
            TrainingUnitElementMasterObject[] result = list.ToArray();
            unitIdCache.Add(unitId, result);
            return result;
        }        
    }
}
