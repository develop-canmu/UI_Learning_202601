using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class BaseCharacterSortFilterModal : SortFilterBaseModal<BaseCharacterSortData, BaseCharacterFilterData>
    {

        [Serializable]
        public class FilterRarityToggleInfo
        {
            public int Rarity;
            public Toggle ToggleObject;
        }
        
        [Serializable]
        public class FilterTypeToggleInfo
        {
            public CharacterType Type;
            public Toggle ToggleObject;
        }

        [Header("絞り込み")]
        [SerializeField] private List<FilterRarityToggleInfo> rarityToggleList;
        [SerializeField] private List<FilterTypeToggleInfo> typeToggleList;
        
        
        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        /// <param name="filterData"></param>
        protected override void SetFilterToggleFromData(BaseCharacterFilterData filterData)
        {
            if(filterData == null) return;
            
            // 状態をリセットするためOFF->ONと切り替える
            foreach (var toggle in rarityToggleList)
            {
                toggle.ToggleObject.isOn = filterData.rarityList.Any(data => data == toggle.Rarity);
            }

            foreach (var toggle in typeToggleList)
            {
                toggle.ToggleObject.isOn = filterData.typeList.Any(data => data == toggle.Type);
            }
        }
        
        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        /// <returns></returns>
        protected override BaseCharacterFilterData CreateFilterData()
        {
            BaseCharacterFilterData filterData = new BaseCharacterFilterData();

            if (rarityToggleList.Any(toggle => toggle.ToggleObject.isOn))
            {
                foreach (var toggleInfo in rarityToggleList)
                {
                    if (toggleInfo.ToggleObject.isOn)
                    {
                        filterData.rarityList.Add(toggleInfo.Rarity);
                    }
                }
            }

            if (typeToggleList.Any(toggle => toggle.ToggleObject.isOn))
            {
                foreach (var toggleInfo in typeToggleList)
                {
                    if (toggleInfo.ToggleObject.isOn)
                    {
                        filterData.typeList.Add(toggleInfo.Type);
                    }
                }
            }

            return filterData;
        }
    }
}


