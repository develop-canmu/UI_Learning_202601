using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pjfb
{
    public class UserTitleSortFilterModal : SortFilterBaseModal<UserTitleSortData, UserTitleFilterData>
    {
        [System.Serializable]
        private class UserTitleToggleInfo
        {
            public long rarity;
            public Toggle toggleObject;
        }
        
        [SerializeField]
        private UserTitleToggleInfo[] rarityToggleList;
        
        /// <summary>フィルタデータの作成</summary>
        protected override UserTitleFilterData CreateFilterData()
        {
            UserTitleFilterData result = new UserTitleFilterData();
            foreach (UserTitleToggleInfo toggleInfo in rarityToggleList)
            {
                if (toggleInfo.toggleObject.isOn)
                {
                    result.rarityList.Add(toggleInfo.rarity);
                }
            }
            return result;
        }
        
        /// <summary>トグルの設定</summary>
        protected override void SetFilterToggleFromData(UserTitleFilterData filterData)
        {
            if(filterData == null) return;
            
            foreach (UserTitleToggleInfo toggleInfo in rarityToggleList)
            {
                // フィルタデータに含まれていればONにする
                toggleInfo.toggleObject.SetIsOnWithoutNotify(filterData.rarityList.Contains(toggleInfo.rarity));
            }
        }
    }
}