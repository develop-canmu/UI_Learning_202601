using CruFramework.UI;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    // 対象枠ラベル
    public class SupportEquipmentLotteryTableSlotLabelScrollDynamicItem : ScrollDynamicItem
    {
        public class Data : SupportEquipmentLotteryTableDataScrollDynamicSelector.IScrollItem
        {
            // スロット番号
            private long[] slotNumbers = null;
            public long[] SlotNumbers => slotNumbers;

            public Data(long[] slotNumbers)
            {
                this.slotNumbers = slotNumbers;
            }
        }
        
        // 対象の枠表示テキスト
        [SerializeField]
        private TMP_Text slotNumberText = null;
        
        protected override void OnSetView(object value)
        {
            Data data = (Data)value;
            
            
            string[] slotNumberStrings = new string[data.SlotNumbers.Length];

            // スロット番号を枠目表記に変換
            for (int i = 0; i < data.SlotNumbers.Length; i++)
            {
                slotNumberStrings[i] = string.Format(StringValueAssetLoader.Instance["practice_skill.slot_number"], data.SlotNumbers[i]);
            }
         
            // 枠ごとに区切る
            slotNumberText.text = string.Join("/", slotNumberStrings);
        }
    }
}