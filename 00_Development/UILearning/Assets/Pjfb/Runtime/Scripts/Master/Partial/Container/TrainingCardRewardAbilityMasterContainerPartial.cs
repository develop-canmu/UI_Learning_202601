

using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class TrainingCardRewardAbilityMasterContainer : MasterContainerBase<TrainingCardRewardAbilityMasterObject> {
        long GetDefaultKey(TrainingCardRewardAbilityMasterObject masterObject){
            return masterObject.id;
        }
        
        // キャッシュ
        private Dictionary<long, TrainingCardRewardAbilityMasterObject[]> findByCardIdCache = new Dictionary<long, TrainingCardRewardAbilityMasterObject[]>();

        public TrainingCardRewardAbilityMasterObject[] FindDataByCardId(long cardId)
        {
        
            // キャッシュから取得
            if(findByCardIdCache.TryGetValue(cardId, out TrainingCardRewardAbilityMasterObject[] datas))
            {
                return datas;
            }
        
            List<TrainingCardRewardAbilityMasterObject> list = new List<TrainingCardRewardAbilityMasterObject>();
            foreach(TrainingCardRewardAbilityMasterObject value in values)
            {
                if(value.mTrainingCardId == cardId)
                {
                    list.Add(value);
                }
            }
            
            // 配列化
            TrainingCardRewardAbilityMasterObject[] result = list.ToArray();
            // キャッシュに追加
            findByCardIdCache.Add(cardId, result);
            
            return result;
        }
    }
}
