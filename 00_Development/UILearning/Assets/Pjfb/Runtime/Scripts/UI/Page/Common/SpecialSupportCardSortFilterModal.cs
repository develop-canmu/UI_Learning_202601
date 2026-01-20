using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    
    public class SpecialSupportCardSortFilterModal : SortFilterBaseModal<SpecialSupportCardSortData, SpecialSupportCardFilterData>
    {
        [Serializable]
        public class FilterRarityToggleInfo
        {
            public long Rarity;
            public SortFilterToggle ToggleObject;
        }
        
        [Serializable]
        public class FilterTypeToggleInfo
        {
            public CharacterType Type;
            public Toggle ToggleObject;
        }
        
        [Serializable]
        public class FilterExtraToggleInfo
        {
            public CharacterExtraType ExtraType;
            public Toggle ToggleObject;
        }
        
        [SerializeField]
        private GameObject extraFilterRoot = null;

        [Header("絞り込み")]
        [SerializeField] private List<FilterTypeToggleInfo> typeToggleList;
        [SerializeField] private List<FilterExtraToggleInfo> extraToggleList;

        /// <summary>トグルを生成するRoot</summary>
        [SerializeField]
        private Transform rarityToggleItemRoot = null;
        
        /// <summary>レアリティ項目のトグルプレハブ</summary>
        [SerializeField]
        private SortFilterToggle rarityTogglePrefab = null;
        
        /// <summary>レアリティ項目のトグルリスト</summary>
        private List<FilterRarityToggleInfo> rarityToggleList = new List<FilterRarityToggleInfo>();

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            if(args is Data arguments)
            {
                extraFilterRoot.SetActive( (arguments.options & SortFilterModalOption.HideExtraFilter) == SortFilterModalOption.None );
            }
            
            return base.OnPreOpen(args, token);
        }

        /// <summary>マスタ参照でレアリティ項目のトグルを生成する</summary>
        protected override void CreateToggleListFromMasterData()
        {
            // 既に生成済みの場合はスキップ
            if (rarityToggleList.Count > 0)
            {
                return;
            }

            // マスタからサポカの表示可能なレアリティだけのデータに絞り込む
            IOrderedEnumerable<RarityMasterObject> rarityMasterObjectList = MasterManager.Instance.rarityMaster.values
                .Where(data => AppTime.IsInPeriod(data.displayStartAt.TryConvertToDateTime()))
                .Where(data => data.cardType == (int)CardType.SpecialSupportCharacter)
                .OrderBy(data => data.value);

            // 表示可能なレアリティデータリストからトグルにデータをセットする
            foreach (RarityMasterObject rarityMasterObject in rarityMasterObjectList)
            {
                FilterRarityToggleInfo rarityToggleInfo = new FilterRarityToggleInfo();
                // レアリティを設定
                rarityToggleInfo.Rarity = rarityMasterObject.value;
                
                // レアリティに紐づくToggleを生成してデータセットする
                rarityToggleInfo.ToggleObject = Instantiate(rarityTogglePrefab, rarityToggleItemRoot);
                rarityToggleInfo.ToggleObject.SetName(rarityMasterObject.name);
                
                // 生成したToggleを表示する
                rarityToggleInfo.ToggleObject.gameObject.SetActive(true);
                
                // 生成したToggleをリストに追加
                rarityToggleList.Add(rarityToggleInfo);
            }
        }

        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        /// <param name="filterData"></param>
        protected override void SetFilterToggleFromData(SpecialSupportCardFilterData filterData)
        {
            if(filterData == null) return;

            // 状態をリセットするためOFF->ONと切り替える
            foreach (var toggle in rarityToggleList)
            {
                toggle.ToggleObject.SetIsOnWithoutNotify(filterData.rarityList.Any(data => data == toggle.Rarity));
            }

            foreach (var toggle in typeToggleList)
            {
                toggle.ToggleObject.SetIsOnWithoutNotify(filterData.typeList.Any(data => data == toggle.Type));
            }
            
            foreach (var toggle in extraToggleList)
            {
                toggle.ToggleObject.SetIsOnWithoutNotify(filterData.extraList.Any(data => data == toggle.ExtraType));
            }
        }
        
        
        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        /// <returns></returns>
        protected override SpecialSupportCardFilterData CreateFilterData()
        {
            SpecialSupportCardFilterData filterData = new SpecialSupportCardFilterData();

            // レアリティのトグル
            foreach (FilterRarityToggleInfo toggleInfo in rarityToggleList)
            {
                // ONのものを取得してリストに追加
                if (toggleInfo.ToggleObject.IsOn)
                {
                    filterData.rarityList.Add(toggleInfo.Rarity);
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
            
            if (extraToggleList.Any(toggle => toggle.ToggleObject.isOn))
            {
                foreach (var toggleInfo in extraToggleList)
                {
                    if (toggleInfo.ToggleObject.isOn)
                    {
                        filterData.extraList.Add(toggleInfo.ExtraType);
                    }
                }
            }

            return filterData;
        }
    }
}


