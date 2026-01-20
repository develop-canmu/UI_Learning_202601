using CruFramework.UI;
using Pjfb;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> 抽選テーブル情報表示アイテム </summary>
    public class SupportEquipmentLotteryTableDataScrollDynamicItem : ScrollDynamicItem
    {
        public class Data : SupportEquipmentLotteryTableDataScrollDynamicSelector.IScrollItem
        {
            private PracticeSkillValueRangeInfo rangeInfo;
            public PracticeSkillValueRangeInfo RangeInfo => rangeInfo;

            // レアリティId
            private long rarityId;
            public long RarityId => rarityId;
            
            public Data(PracticeSkillValueRangeInfo rangeInfo, long rarityId)
            {
                this.rangeInfo = rangeInfo;
                this.rarityId = rarityId;
            }
        }

        // テーブル情報View
        [SerializeField]
        private PracticeSkillViewMini skillView;

        protected override void OnSetView(object value)
        {
            Data data = (Data)value;
            skillView.SetSkillData(data.RangeInfo, data.RarityId);
        }
    }
}
