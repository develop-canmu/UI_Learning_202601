
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class TrainingConcentrationLotteryMasterContainer : MasterContainerBase<TrainingConcentrationLotteryMasterObject> {
        long GetDefaultKey(TrainingConcentrationLotteryMasterObject masterObject){
            return masterObject.id;
        }

        /// <summary> そのキャラクターが使用するコンセントレーションゾーンIdリスト </summary>
        public long[] GetTrainingCharacterConcentrationIdList(long trainingScenarioId, long mCharaId)
        {
            // キャラ共通のコンセントレーションゾーンIDリスト
            List<long> commonIdList = new List<long>();
            // キャラ固有のコンセントレーションゾーンIDリスト
            List<long> characterUniqueIdList = new List<long>();
            
            foreach (var master in values)
            {
                if(master.mTrainingScenarioId != trainingScenarioId)continue;
                
                // キャラ共通
                if(master.mCharaId == 0)
                {
                    // 共通
                    commonIdList.Add(master.mTrainingConcentrationId);
                }
                // キャラ固有
                else if(master.mCharaId == mCharaId)
                {
                    // 固有
                    characterUniqueIdList.Add(master.mTrainingConcentrationId);
                }
            }

            // 固有のコンセントレーションゾーンIdの設定があるなら固有Idで返す
            if (characterUniqueIdList.Count > 0)
            {
                return characterUniqueIdList.ToArray();
            }
            
            // 固有が存在しない場合は共通Idで返す
            return commonIdList.ToArray();
        }
    }
}
