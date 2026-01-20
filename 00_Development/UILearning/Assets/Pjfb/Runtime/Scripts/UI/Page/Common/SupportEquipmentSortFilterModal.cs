using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Extensions;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class SupportEquipmentSortFilterModal : SortFilterBaseModal<SupportEquipmentSortData, SupportEquipmentFilterData>
    {
        [Serializable]
        public class FilterIconTypeToggleInfo
        {
            [SerializeField]
            private long iconType;
            public long IconType => iconType;
            
            [SerializeField]
            private Toggle toggleObject;
            public Toggle ToggleObject => toggleObject;
        }

        [Serializable]
        public class FilterRarityToggleInfo
        {
            public long Rarity;
            public SortFilterToggle ToggleObject;
        }
        
        [Serializable]
        public class FilterTypeToggleInfo
        {
            public long Type;
            public Toggle ToggleObject;
        }

        [Serializable]
        public class FilterPracticeSkillInfo
        {
            public long PracticeSkillId;
            public TMP_Text TextObject;
        }

        /// <summary>ブランクテキスト</summary>
        private const string BlankKey = "pvp.blank";
        
        [Header("絞り込み")]
        [SerializeField] private List<FilterIconTypeToggleInfo> iconTypeToggleList;
        [SerializeField] private List<FilterTypeToggleInfo> typeToggleList;
        [SerializeField] private List<FilterPracticeSkillInfo> practiceSkillTextList;

        // レアリティの生成位置
        [SerializeField] 
        private Transform rarityToggleRoot = null;

        // レアリティトグルで生成するオブジェクト
        [SerializeField] 
        private SortFilterToggle rarityTogglePrefab = null;

        // 生成されたレアリティトグルリスト
        private List<FilterRarityToggleInfo> rarityToggleList = new List<FilterRarityToggleInfo>();
        
        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        /// <param name="filterData"></param>
        protected override void SetFilterToggleFromData(SupportEquipmentFilterData filterData)
        {
            if(filterData == null) return;

            // アイコンタイプ
            foreach (FilterIconTypeToggleInfo toggleInfo in iconTypeToggleList)
            {
                toggleInfo.ToggleObject.SetIsOnWithoutNotify(filterData.iconTypeList.Any(data => data == toggleInfo.IconType));
            }
            
            // レアリティ
            foreach (var toggle in rarityToggleList)
            {
                toggle.ToggleObject.SetIsOnWithoutNotify(filterData.rarityList.Any(data => data == toggle.Rarity));
            }

            // 種類
            foreach (var toggle in typeToggleList)
            {
                toggle.ToggleObject.SetIsOnWithoutNotify(filterData.typeList.Any(data => data == toggle.Type));
            }

            // 練習能力
            UpdatePracticeSkillList(filterData.practiceSkillList);
        }

        /// <summary> マスターからのトグル作成 </summary>
        protected override void CreateToggleListFromMasterData()
        {
            // すでに生成済みならリターン
            if (rarityToggleList.Count > 0)
            {
                return;
            }
            
            IOrderedEnumerable<RarityMasterObject> rarityMasterList = MasterManager.Instance.rarityMaster.values
                // サポート器具
                .Where(x => x.cardType == (int)CardType.SupportEquipment)
                // 表示開始時間を超えている
                .Where(x => AppTime.IsInPeriod(x.displayStartAt.TryConvertToDateTime()))
                // 昇順でソート
                .OrderBy(x => x.value);

            // 見つかったマスターからトグルを生成する
            foreach (RarityMasterObject master in rarityMasterList)
            {
                // トグルを生成
                SortFilterToggle rarityToggle = Instantiate(rarityTogglePrefab, rarityToggleRoot);
                // 表示名設定
                rarityToggle.SetName(master.name);
                // オブジェクトをアクティブに
                rarityToggle.gameObject.SetActive(true);
                FilterRarityToggleInfo rarityInfo = new FilterRarityToggleInfo();
                rarityInfo.Rarity = master.value;
                rarityInfo.ToggleObject = rarityToggle;
                
                // リストに追加
                rarityToggleList.Add(rarityInfo);
            }
        }

        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        /// <returns></returns>
        protected override SupportEquipmentFilterData CreateFilterData()
        {
            SupportEquipmentFilterData filterData = new SupportEquipmentFilterData();

            // アイコンタイプ
            foreach (FilterIconTypeToggleInfo toggleInfo in iconTypeToggleList)
            {
                if (toggleInfo.ToggleObject.isOn)
                {
                    filterData.iconTypeList.Add(toggleInfo.IconType);
                }
            }

            // レアリティ
            foreach (var toggleInfo in rarityToggleList)
            {
                if (toggleInfo.ToggleObject.IsOn)
                {
                    filterData.rarityList.Add(toggleInfo.Rarity);
                }
            }

            // 種類
            foreach (var toggleInfo in typeToggleList)
            {
                if (toggleInfo.ToggleObject.isOn)
                {
                    filterData.typeList.Add(toggleInfo.Type);
                }
            }

            // 練習能力
            foreach (var practiceSkillInfo in practiceSkillTextList)
            {
                if (practiceSkillInfo.PracticeSkillId != 0)
                {
                    filterData.practiceSkillList.Add(MasterManager.Instance.trainingStatusTypeDetailMaster
                        .FindData(practiceSkillInfo.PracticeSkillId).id);
                }
            }



            return filterData;
        }

        /// <summary>選択中の練習能力の表示更新</summary>
        private void UpdatePracticeSkillList(List<long> practiceSkillList)
        {
            int count = 0;
            foreach (var practiceSkillInfo in practiceSkillTextList)
            {
                if (practiceSkillList != null && practiceSkillList.Count > count && practiceSkillList[count] != 0)
                {
                    var master = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(practiceSkillList[count]);
                    practiceSkillInfo.PracticeSkillId = master.id;
                    practiceSkillInfo.TextObject.SetText(master.name);
                }
                else
                {
                    practiceSkillInfo.PracticeSkillId = 0;
                    practiceSkillInfo.TextObject.SetText(StringValueAssetLoader.Instance[BlankKey]);
                }

                count++;
            }
        }
        public async void OnClickPracticeSkillSelectButton()
        {
            CruFramework.Page.ModalWindow m = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SupportEquipmentFilterPracticeSkillSelection, practiceSkillTextList);
            var closeParameter = (List<long>)await m.WaitCloseAsync();
            UpdatePracticeSkillList(closeParameter);
        }
    }
}


