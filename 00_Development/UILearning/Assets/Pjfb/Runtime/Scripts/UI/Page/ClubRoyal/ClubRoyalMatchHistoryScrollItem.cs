using CruFramework.UI;
using UnityEngine;

namespace Pjfb.ClubRoyal
{
    /// <summary>試合履歴に表示するアイテム</summary>
    public class ClubRoyalMatchHistoryScrollItem : ScrollGridItem
    {
        /// <summary>試合履歴に表示する各アイテムのView</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryItemView matchHistoryItemView = null;
        
        protected override void OnSetView(object value)
        {
            // 試合履歴のアイテムのViewにデータをセット
            matchHistoryItemView.SetView((ClubRoyalMatchHistoryItemView.MatchHistoryInfo)value);
        }
    }
}