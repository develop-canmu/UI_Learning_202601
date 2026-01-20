using CruFramework.UI;
using Pjfb;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> 抽選テーブル情報表示アイテム </summary>
    public class SupportEquipmentLotteryTableDataScrollDynamicSelector : ScrollDynamicItemSelector
    {
        // 共通アイテムデータを示すインターフェース
        public interface IScrollItem
        {
            
        }
        
        // スロット番号表示ラベル
        [SerializeField]
        private ScrollDynamicItem slotLabel;
        
        // スキル表示
        [SerializeField]
        private ScrollDynamicItem skillItem;

        public override ScrollDynamicItem GetItem(object item)
        {
            // スロット番号表示ラベル
            if (item is SupportEquipmentLotteryTableSlotLabelScrollDynamicItem.Data)
            {
                return slotLabel;
            }
            // スキル表示アイテム
            else
            {
                return skillItem;
            }
        }
    }
}
