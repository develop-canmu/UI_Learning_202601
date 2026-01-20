
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pjfb.Master {

    public partial class CharaTrainingStatusMasterContainer : MasterContainerBase<CharaTrainingStatusMasterObject> {
        long GetDefaultKey(CharaTrainingStatusMasterObject masterObject){
            return masterObject.id;
        }
        
        public bool HasStatus(long mCharId, long lv, long mScenarioId)
        {
            foreach(CharaTrainingStatusMasterObject value in values)
            {
                if(value.level <= lv && value.mCharaId == mCharId && value.mTrainingScenarioId == mScenarioId)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>各キャラクタの最大Lvのスキルを取得</summary>
        public Dictionary<long, CharaTrainingStatusMasterObject> FindLvMaxData(long mScenarioId)
        {
            Dictionary<long, CharaTrainingStatusMasterObject> result = new Dictionary<long, CharaTrainingStatusMasterObject>();

            foreach(CharaTrainingStatusMasterObject value in values)
            {
                if(value.mTrainingScenarioId != mScenarioId)continue;
            
                if( result.TryGetValue( value.mCharaId, out CharaTrainingStatusMasterObject status) == false)
                {
                    result.Add(value.mCharaId, value);
                }
                else
                {
                    if(status.level < value.level)
                    {
                        result[value.mCharaId] = value;
                    }
                }
            }

            return result;
        }
        
        public CharaTrainingStatusMasterObject FindData(long mCharId, long lv, long trainingScenarioId, bool isUnique)
        {
            CharaTrainingStatusMasterObject result = null;
            long level = 0;

            foreach(CharaTrainingStatusMasterObject value in values)
            {
                if(value.isUnique != isUnique)continue;
                // キャラIdが違う
                if(value.mCharaId != mCharId)continue;
                // シナリオIdが違う
                if(value.mTrainingScenarioId != trainingScenarioId)continue;
                // レベルチェック
                if(value.level > lv)continue;
                // 最大レベルのものを取得
                if(level < value.level)
                {
                    level = value.level;
                    result = value;
                }
            }
            
            return result;
        }
        
        public CharaTrainingStatusMasterObject FindData(long mCharId, long lv, bool isUnique)
        {
            CharaTrainingStatusMasterObject result = null;
            long level = 0;
            
            foreach(CharaTrainingStatusMasterObject value in values)
            {
                if(value.isUnique != isUnique)continue;
                // キャラIdが違う
                if (value.mCharaId != mCharId) continue;
                // レベルチェック
                if (value.level == lv)
                {
                    result = value;
                    return result;
                }
                
                // 設定上一致するレベルが無い場合はキャラ同様に一致するmCharaIdのうち有効なレベルのうち最大レベルの効果量を取得
                // レベルチェック
                if (value.level > lv) continue;
                // 最大レベルのものを取得
                if (level < value.level)
                {
                    level = value.level;
                    result = value;
                }
                
            }
            
            return result;
        }
    }
}
