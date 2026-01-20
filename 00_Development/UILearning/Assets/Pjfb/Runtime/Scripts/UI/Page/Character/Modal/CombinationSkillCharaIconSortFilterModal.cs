using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Extensions;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Pjfb.Combination
{
    public class CombinationSkillCharaIconSortFilterModal : SortFilterBaseModal<CombinationSkillCharaIconSortData, CombinationSkillCharaIconFilterData>
    {
        // 継承したDataクラス
        public class CombinationSkillCharaIconData : Data
        {
            public CardType cardType;
    
            public CombinationSkillCharaIconData(SortFilterSheetType sheetType, SortFilterUtility.SortFilterType sortFilterType, CardType cardType, SortFilterModalOption options = SortFilterModalOption.None)
                : base(sheetType, sortFilterType, options)
            {
                this.cardType = cardType;
            }
        }
        private CombinationSkillCharaIconData combinationModalData;
        
        /// <summary>
        /// レアリティのフィルターとトグルをまとめたクラス
        /// </summary>
        [Serializable]
        private class FilterRarityToggleInfo
        {
            public long rarity;
            public SortFilterToggle toggleObject;
        }
    
        [Header("絞り込み")]
        // トグルを生成するルートオブジェクト
        [SerializeField] private Transform rarityToggleItemRoot;
        // レアリティトグルプレハブ
        [SerializeField] private SortFilterToggle rarityTogglePrefab;
        
        // レアリティトグル
        private List<FilterRarityToggleInfo> rarityToggleList = new();

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            combinationModalData = (CombinationSkillCharaIconData)args;
            
            return base.OnPreOpen(args, token);
        }
        
        // サポカのレアリティトグルはマスタから取得するため、トグル生成処理を実装
        protected override void CreateToggleListFromMasterData()
        {
            // Serializeで設定されている場合はスキップ
            if (rarityToggleList.Count > 0)
            {
                return;
            }
            
            // マスタから表示可能なレアリティだけのデータに絞り込む
            IOrderedEnumerable<RarityMasterObject> rarityMasterObjectList = MasterManager.Instance.rarityMaster.values
                .Where(x => x.cardType == (int)combinationModalData.cardType)
                .Where(x => AppTime.IsInPeriod(x.displayStartAt.TryConvertToDateTime()))
                .OrderBy(x => x.value);
            
            // レアリティトグルにデータをセット
            foreach (RarityMasterObject rarityMasterObject in rarityMasterObjectList)
            {
                FilterRarityToggleInfo rarityToggleInfo = new();
                // レアリティ設定
                rarityToggleInfo.rarity = rarityMasterObject.value;
                
                // トグル生成
                rarityToggleInfo.toggleObject = Instantiate(rarityTogglePrefab, rarityToggleItemRoot);
                rarityToggleInfo.toggleObject.SetName(rarityMasterObject.name);
                rarityToggleInfo.toggleObject.gameObject.SetActive(true);
                // リストに追加
                rarityToggleList.Add(rarityToggleInfo);
            }
        }
        
        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        protected override void SetFilterToggleFromData(CombinationSkillCharaIconFilterData filterData)
        {
            if(filterData == null) return;
            
            // 状態をリセットするためOFF->ONと切り替える
            foreach (var toggle in rarityToggleList)
            {
                toggle.toggleObject.SetIsOnWithoutNotify(filterData.selectedRarityList.Any(data => data == toggle.rarity));
            }
        }
        
        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        /// <returns></returns>
        protected override CombinationSkillCharaIconFilterData CreateFilterData()
        {
            CombinationSkillCharaIconFilterData filterData = new CombinationSkillCharaIconFilterData();

            if (rarityToggleList.Any(toggle => toggle.toggleObject.IsOn))
            {
                foreach (var toggleInfo in rarityToggleList)
                {
                    if (toggleInfo.toggleObject.IsOn)
                    {
                        filterData.selectedRarityList.Add(toggleInfo.rarity);
                    }
                }
            }

            return filterData;
        }
    }
}
