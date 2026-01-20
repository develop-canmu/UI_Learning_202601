using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public abstract class CombinationViewBase<TPrefab, TSkill> : MonoBehaviour where TPrefab : MonoBehaviour
    {
        public enum NotActiveLabelType
        {
            Next,
            Total
        }
        
        [SerializeField] private ScrollGrid charaScroll;
        [SerializeField] private Transform activatedSkillRoot;
        [SerializeField] private TPrefab activatedSkillPrefab;
        [SerializeField] private TMPro.TMP_Text notActiveLabelText;
        [SerializeField] private Transform notActiveSkillRoot;
        [SerializeField] private TPrefab notActiveSkillPrefab;
        
        protected List<TPrefab> ActivatedSkillPrefabList = new();
        protected List<TPrefab> notActiveSkillPrefabList = new();

        protected void InitializeCharaUI(List<CharacterDetailData> characterDetailDataList, IReadOnlyDictionary<long, UserDataChara> mCharaIdPossessionDictionary, bool showCharaLevel)
        {
            // Character
            List<CombinationCharacterScrollData> charaScrollDataList = characterDetailDataList.Select(characterDetail =>
                new CombinationCharacterScrollData(characterDetail, mCharaIdPossessionDictionary, showCharaLevel)).ToList();
            charaScroll.SetItems(charaScrollDataList);
        }
        
        protected void InitializeSkillUi(List<TSkill> activatedSkillDataList, List<TSkill> lockSkillDataList, 
            NotActiveLabelType notActiveLabelType)
        {
            // 解放済みスキル表示
            InitializeCollectionMultipleSkillPrefab(ActivatedSkillPrefabList, activatedSkillDataList, activatedSkillPrefab,
                activatedSkillRoot);

            // 未解放スキル表示
            switch (notActiveLabelType)
            {
                case NotActiveLabelType.Next:
                    notActiveLabelText.text = StringValueAssetLoader.Instance["combination.cell.skill.label.not_active_next"];
                    break;
                case NotActiveLabelType.Total:
                    notActiveLabelText.text = StringValueAssetLoader.Instance["combination.cell.skill.label.not_active_total"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notActiveLabelType), notActiveLabelType, null);
            }
            InitializeCollectionMultipleSkillPrefab(notActiveSkillPrefabList, lockSkillDataList, notActiveSkillPrefab,
                notActiveSkillRoot);
        }
        
        protected void InitializeCollectionMultipleSkillPrefab(List<TPrefab> prefabList, List<TSkill> multipleSkillDataList, TPrefab tPrefab, Transform prefabRoot)
        {
            var hasSkillData = multipleSkillDataList.Count > 0;
            
            // データがない場合親オブジェクトを非表示にする
            prefabRoot.gameObject.SetActive(hasSkillData);

            // notActiveSkillPrefabListが0以下の場合既に配置してあるprefabを入れておく
            if (prefabList.Count <= 0)
            {
                prefabList.Add(tPrefab);
            }
            
            // 解放可能スキル表示
            for (int i = prefabList.Count; i < multipleSkillDataList.Count; i++)
            {
                var prefab = Instantiate(tPrefab, prefabRoot);
                prefabList.Add(prefab);
            }
            
            for (int i = 0; i < prefabList.Count; i++)
            {
                var prefab = prefabList[i];
                if (i >= multipleSkillDataList.Count)
                {
                    prefab.gameObject.SetActive(false);
                    continue;
                }
                prefab.gameObject.SetActive(true);
                var multipleSkillData = multipleSkillDataList[i];
                InitializeSkillPrefab(prefab, multipleSkillData);
            }
        }

        protected abstract void InitializeSkillPrefab(TPrefab tPrefab, TSkill tSkill);
    }
}
