using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb
{
    public class UserIconSortFilterModal : SortFilterBaseModal<UserIconSortData, UserIconFilterData>
    {
        [System.Serializable]
        private class FilterRarityToggleInfo
        {
            public long Rarity;
            public Toggle ToggleObject;
        }
        
        [SerializeField]
        private List<FilterRarityToggleInfo> rarityToggleList;
        
        /// <summary>フィルタデータの作成</summary>
        protected override UserIconFilterData CreateFilterData()
        {
            UserIconFilterData result = new UserIconFilterData();
            foreach (FilterRarityToggleInfo toggleInfo in rarityToggleList)
            {
                if (toggleInfo.ToggleObject.isOn)
                {
                    result.rarityList.Add(toggleInfo.Rarity);
                }
            }
            return result;
        }

        /// <summary>トグルの設定</summary>
        protected override void SetFilterToggleFromData(UserIconFilterData filterData)
        {
            if(filterData == null) return;
            
            foreach (FilterRarityToggleInfo toggleInfo in rarityToggleList)
            {
                // フィルタデータに含まれていればONにする
                toggleInfo.ToggleObject.SetIsOnWithoutNotify(filterData.rarityList.Contains(toggleInfo.Rarity));
            }
        }
    }
}