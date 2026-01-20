using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class SuccessCharacterSortFilterModal : SortFilterBaseModal<SuccessCharacterSortData, SuccessCharacterFilterData>
    {
        [Serializable]
        public class FilterRankToggleInfo
        {
            public long Rank;
            public SuccessCharacterSortFilterRankToggle ToggleObject;
        }
        
        [Serializable]
        public class FilterFavoriteToggleInfo
        {
            public SuccessCharacterFilterData.FavoriteType Type;
            public Toggle ToggleObject;
        }

        [Header("絞り込み")]
        [SerializeField] private Transform rankToggleParent;
        [SerializeField] private SuccessCharacterSortFilterRankToggle rankTogglePrefab;
        [SerializeField] private List<FilterFavoriteToggleInfo> favoriteToggleList;
        
        private List<FilterRankToggleInfo> rankToggleList = new ();
        
        /// <summary>マスタを参照して、選手の戦力ランクのトグルを生成</summary>
        protected override void CreateToggleListFromMasterData()
        {
            if(rankToggleList.Count > 0) return;
            foreach (CharaRankMasterObject charaRankMasterObject in MasterManager.Instance.charaRankMaster.values.OrderByDescending(data => data.id))
            {
                // キャラ総合値のみを表示
                if(charaRankMasterObject.type != (int)CharaRankMasterStatusType.CharacterTotal) continue;
                
                FilterRankToggleInfo rankToggleInfo = new ();
                rankToggleInfo.Rank = charaRankMasterObject.rankNumber;
                rankToggleInfo.ToggleObject = Instantiate(rankTogglePrefab, rankToggleParent);
                rankToggleInfo.ToggleObject.SetName(charaRankMasterObject.name);
                rankToggleInfo.ToggleObject.gameObject.SetActive(true);
                
                rankToggleList.Add(rankToggleInfo);
            }
        }

        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        /// <param name="filterData"></param>
        protected override void SetFilterToggleFromData(SuccessCharacterFilterData filterData)
        {
            if (filterData == null) return;
            
            foreach (var rankToggleInfo in rankToggleList)
            {
                rankToggleInfo.ToggleObject.SetIsOnWithoutNotify(filterData.rankList.Any(data => data == rankToggleInfo.Rank));
            }

            foreach (var favoriteToggleInfo in favoriteToggleList)
            {
                favoriteToggleInfo.ToggleObject.SetIsOnWithoutNotify(favoriteToggleInfo.Type == filterData.favoriteType);
            }
        }
        
        
        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        /// <returns></returns>
        protected override SuccessCharacterFilterData CreateFilterData()
        {
            SuccessCharacterFilterData filterData = new SuccessCharacterFilterData();

            foreach (var toggleInfo in rankToggleList)
            {
                if (toggleInfo.ToggleObject.IsOn)
                {
                    filterData.rankList.Add(toggleInfo.Rank);
                }
            }

            filterData.favoriteType = favoriteToggleList.FirstOrDefault(toggle => toggle.ToggleObject.isOn)?.Type ?? SuccessCharacterFilterData.FavoriteType.All;

            return filterData;
        }
    }
}


