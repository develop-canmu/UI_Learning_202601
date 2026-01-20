using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class MyBadgeSortFilterModal : SortFilterBaseModal<MyBadgeSortData, MyBadgeFilterData>
    {
        [Serializable]
        private class ToggleInfo
        {
            public long rarityId;
            public Toggle toggle;
        }
        
        [SerializeField]
        private ToggleInfo[] toggleInfo;
        
        protected override MyBadgeFilterData CreateFilterData()
        {
            var result = new MyBadgeFilterData();

            foreach (ToggleInfo info in toggleInfo)
            {
                if (info.toggle.isOn)
                {
                    result.rarityList.Add(info.rarityId);
                }
            }

            return result;
        }

        protected override void SetFilterToggleFromData(MyBadgeFilterData filterData)
        {
            foreach (ToggleInfo info in toggleInfo)
            {
                info.toggle.isOn = filterData.rarityList.Contains(info.rarityId);
            }
        }
    }
}