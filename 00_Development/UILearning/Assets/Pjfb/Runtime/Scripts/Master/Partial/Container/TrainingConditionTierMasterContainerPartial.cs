
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {
    
    public partial class TrainingConditionTierMasterContainer : MasterContainerBase<TrainingConditionTierMasterObject> {
        
        // ScenarioIdごとのTierListキャッシュ
        Dictionary<long, TrainingConditionTierMasterObject[]> cacheTierList = new Dictionary<long, TrainingConditionTierMasterObject[]>();
        
        long GetDefaultKey(TrainingConditionTierMasterObject masterObject){
            return masterObject.id;
        }

        /// <summary> 一致するシナリオIdのマスタのデータリストを取得 </summary>
        public TrainingConditionTierMasterObject[] FindTierSortedList(long trainingScenarioId)
        {
            // キャッシュ済み
            if(cacheTierList.TryGetValue(trainingScenarioId, out TrainingConditionTierMasterObject[] tierList))
            {
                return tierList;
            }

            List<TrainingConditionTierMasterObject> result = new List<TrainingConditionTierMasterObject>();
            
            foreach (TrainingConditionTierMasterObject master in values)
            {
                if (master.mTrainingScenarioId != trainingScenarioId)
                {
                    continue;
                }
                
                result.Add(master);
            }

            // Tier降順にソート
            TrainingConditionTierMasterObject[] sortedList = result.OrderByDescending(x => x.tier).ToArray();
            // キャッシュ登録
            cacheTierList.Add(trainingScenarioId, sortedList);
            
            return sortedList;
        }
    }
}
