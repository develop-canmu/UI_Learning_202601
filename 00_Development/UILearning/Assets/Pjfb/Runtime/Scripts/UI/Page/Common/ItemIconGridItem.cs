using CruFramework.UI;
using UnityEngine;

namespace Pjfb
{ 
    public class ItemIconGridItem : ScrollGridItem
    {
        public class Data
        {
            public long MPointId;
            public long ItemCount;
            public long RequiredValue;
            public long PossessionValue;
            public bool IsDigitString;

            public Data(long mPointId, long count, bool isDigitString = false)
            {
                MPointId = mPointId;
                ItemCount = count;
                IsDigitString = isDigitString;
                
                RequiredValue = PossessionValue = -1;
            }
            
            public Data(long _mPointId, long _requiredValue, long _possessionValuevalue, bool isDigitString = false)
            {
                MPointId = _mPointId;
                RequiredValue = _requiredValue;
                PossessionValue = _possessionValuevalue;
                IsDigitString = isDigitString;

                ItemCount = -1;
            }
        }
        
        #region SerializeFields
        [SerializeField] private ItemIcon itemIcon;
        #endregion
        
        public Data ItemIconGridItemData;
        
        protected override void OnSetView(object value)
        {
            ItemIconGridItemData = (Data)value;
            itemIcon.SetIconId(ItemIconGridItemData.MPointId);
            if (ItemIconGridItemData.IsDigitString)
            {
                if (ItemIconGridItemData.ItemCount >= 0)
                {
                    itemIcon.SetCountDigitString(ItemIconGridItemData.ItemCount);
                }
                else
                {
                    itemIcon.SetCountDigitString(ItemIconGridItemData.PossessionValue, ItemIconGridItemData.RequiredValue);    
                }
                
            }
            else
            {
                if (ItemIconGridItemData.ItemCount >= 0)
                {
                    itemIcon.SetCount(ItemIconGridItemData.ItemCount);
                }
                else
                {
                    itemIcon.SetCount(ItemIconGridItemData.PossessionValue, ItemIconGridItemData.RequiredValue);    
                }
                
            }
        }

        public void SetItemData(object value)
        {
            OnSetView(value);
        }
    }
}