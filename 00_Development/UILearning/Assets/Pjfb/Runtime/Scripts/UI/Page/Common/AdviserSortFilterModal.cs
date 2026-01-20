using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb
{
    public class AdviserSortFilterModal : SortFilterBaseModal<AdviserSortData, AdviserFilterData>
    {
        // レアリティトグル
        [Serializable]
        private class FilterRarityToggleInfo
        {
            public int Rarity;
            public Toggle ToggleObject;
        }
        
        // タイプトグル
        [Serializable]
        public class FilterTypeToggleInfo
        {
            public CharacterType Type;
            public Toggle ToggleObject;
        }

        // レアリティトグルリスト
        [SerializeField]
        private List<FilterRarityToggleInfo> rarityToggleList;

        // タイプトグルリスト
        [SerializeField]
        private List<FilterTypeToggleInfo> typeToggleList;
        
        protected override AdviserFilterData CreateFilterData()
        {
            AdviserFilterData filterData = new AdviserFilterData();

            // レアリティの絞り込み設定を追加
            if (rarityToggleList.Any(toggle => toggle.ToggleObject.isOn))
            {
                foreach (FilterRarityToggleInfo toggleInfo in rarityToggleList)
                {
                    if (toggleInfo.ToggleObject.isOn)
                    {
                        filterData.rarityList.Add(toggleInfo.Rarity);
                    }
                }
            }
            
            // タイプの絞り込み設定を追加
            if (typeToggleList.Any(toggle => toggle.ToggleObject.isOn))
            {
                foreach (FilterTypeToggleInfo toggleInfo in typeToggleList)
                {
                    if (toggleInfo.ToggleObject.isOn)
                    {
                        filterData.typeList.Add(toggleInfo.Type);
                    }
                }
            }

            return filterData;
        }

        protected override void SetFilterToggleFromData(AdviserFilterData filterData)
        {
            if(filterData == null) return;
            
            // 状態をリセットするためOFF->ONと切り替える
            foreach (FilterRarityToggleInfo toggle in rarityToggleList)
            {
                toggle.ToggleObject.isOn = filterData.rarityList.Any(data => data == toggle.Rarity);
            }

            // タイプトグルのセット
            foreach (FilterTypeToggleInfo toggle in typeToggleList)
            {
                toggle.ToggleObject.isOn = filterData.typeList.Any(data => data == toggle.Type);
            }
        }
    }
}