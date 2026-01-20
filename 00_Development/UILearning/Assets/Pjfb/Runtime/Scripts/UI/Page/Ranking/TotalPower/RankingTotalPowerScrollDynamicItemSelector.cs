using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class RankingTotalPowerScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        /// <summary>スクロール領域に表示するアイテムのプレハブ</summary>
        [SerializeField]
        private ScrollDynamicItem rankingItemPrefab = null;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            // プレハブをそのまま返す
            return rankingItemPrefab;
        }
    }
}