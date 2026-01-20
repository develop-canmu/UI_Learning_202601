using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    // 強化に必要なポイント情報クラス
    public class EnhancePointInfo
    {
        // mEnhanceId
        private long enhanceId = 0;
        public long EnhanceId {get => enhanceId;}
        
        private Dictionary<long, List<EnhanceLevelPointMasterObject>> enhancePointList;
        public Dictionary<long, List<EnhanceLevelPointMasterObject>> EnhancePointList {get => enhancePointList;}

        public EnhancePointInfo(long enhanceId)
        {
            var deckEnhancePointList = MasterManager.Instance.enhanceLevelPointMaster.values.Where(x => x.mEnhanceId == enhanceId)
                .OrderBy(x => x.level)
                .ThenBy(x => x.id);
            
            this.enhanceId = enhanceId;
            // Levelでグループ化してLevelをKeyにもつDictionaryを生成
            enhancePointList = deckEnhancePointList.GroupBy(x => x.level).ToDictionary(x => x.Key, x => x.ToList());
        }
        
        //// <summary> 強化に必要になるポイントの合計値をDictionaryで返す </summary>
        public Dictionary<long, long> GetTotalRequiredCost(long currentLevel, long afterLevel)
        {
            // 現在のレベルよりも大きく強化後のレベル以下でデータをフィルタ
            var filterEnhanceDataList = EnhancePointList.Where(x => x.Key > currentLevel && x.Key <= afterLevel);

            // KeyにPointId、Valueに必要数
            var totalRequiredCost = new Dictionary<long, long>();
            foreach (var enhanceData in filterEnhanceDataList)
            {
                foreach (var enhanceLevelMaster in enhanceData.Value)
                {
                    // まだ追加されていないポイントIdなら追加
                    if (totalRequiredCost.ContainsKey(enhanceLevelMaster.mPointId) == false)
                    {
                        totalRequiredCost.Add(enhanceLevelMaster.mPointId, enhanceLevelMaster.value);
                    }
                    // すでに追加されているなら加算する
                    else
                    {
                        totalRequiredCost[enhanceLevelMaster.mPointId] += enhanceLevelMaster.value;
                    }
                }
            }

            return totalRequiredCost;
        }
    }
    
    public static class EnhancePointUtility
    {
        //// <summary> ポイントが足りているか </summary>
        public static bool IsEnoughPoint(Dictionary<long, long> costList)
        {
            foreach (var enhanceData in costList)
            {
                // 現在のポイントの所持数(nullなら0に)
                long possessionValue = UserDataManager.Instance.point.Find(enhanceData.Key)?.value ?? 0L;
                // ポイントが足りていない
                if (enhanceData.Value > possessionValue)
                {
                    return false;
                }
            }

            return true;
        }
    }
}