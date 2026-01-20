
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class TrainingPointStatusEffectCharaMasterContainer : MasterContainerBase<TrainingPointStatusEffectCharaMasterObject> {
        long GetDefaultKey(TrainingPointStatusEffectCharaMasterObject masterObject){
            return masterObject.id;
        }
        
        // mCharaIDとレベルをキーにしたキャッシュ用の辞書
        Dictionary<long,Dictionary<long,TrainingPointStatusEffectCharaMasterObject>> cashDataDictionary = new Dictionary<long, Dictionary<long, TrainingPointStatusEffectCharaMasterObject>>();
        
        public TrainingPointStatusEffectCharaMasterObject GetDataByCharaData(long mCharaId, long level)
        {
            // キャッシュから取得
            if(cashDataDictionary.ContainsKey(mCharaId))
            {
                if(cashDataDictionary[mCharaId].ContainsKey(level))
                {
                    return cashDataDictionary[mCharaId][level];
                }
            }
            
            foreach(TrainingPointStatusEffectCharaMasterObject value in values)
            {
                // キャラIDが違う場合continue
                if(value.mCharaId != mCharaId) continue;
                // レベルが違う場合continue
                if(value.level != level) continue;
                
                // キャッシュに追加
                if(cashDataDictionary.ContainsKey(mCharaId) == false)
                {
                    cashDataDictionary[mCharaId] = new Dictionary<long, TrainingPointStatusEffectCharaMasterObject>();
                }
                cashDataDictionary[mCharaId][level] = value;
                
                return value;
            }
            return null;
        }
    }
}
