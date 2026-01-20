using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    public class SupportEquipmentIconScrollData
    {
        public SupportEquipmentDetailData DetailData { get; }
        public SwipeableParams<SupportEquipmentDetailData> SwipeableParams { get; }

        public SupportEquipmentIconScrollData(SupportEquipmentDetailData detailData, SwipeableParams<SupportEquipmentDetailData> swipeableParams)
        {
            DetailData = detailData;
            SwipeableParams = swipeableParams;
        }
    }
    
    public class SupportEquipmentIconScrollItem : ScrollGridItem
    {
        private SupportEquipmentIconScrollData scrollData;
        
        [SerializeField]
        private SupportEquipmentIcon icon = null;
        
        protected override void OnSetView(object value)
        {
            scrollData = (SupportEquipmentIconScrollData)value;
            icon.SetIconAsync(scrollData.DetailData).Forget();
            icon.SwipeableParams = scrollData.SwipeableParams;

        }
    }
}