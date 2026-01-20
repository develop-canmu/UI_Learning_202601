using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Combination;

namespace Pjfb
{
    public class CombinationMatchView : MonoBehaviour
    {
        [SerializeField] private List<CombinationCharacterVariableIcon> characterIconList;
        [SerializeField] private Transform skillRoot;
        [SerializeField] private CharacterDetailSkillView skillPrefab;

        private List<CharacterDetailSkillView> skillPrefabList = new();

        public void Initialize(CombinationManager.CombinationMatch combinationMatch)
        {
            // Character
            for (int i = 0; i < characterIconList.Count; i++)
            {
                if (i >= combinationMatch.MCharaComboAbilityElementSortedList.Count)
                {
                    characterIconList[i].gameObject.SetActive(false);
                    continue;
                }
                characterIconList[i].gameObject.SetActive(true);
                characterIconList[i].Initialize(combinationMatch.MCharaComboAbilityElementSortedList[i].mCharaId,
                    combinationMatch.MCharaComboAbilityElementSortedList[i].isTarget);

            }

            InitializeSkill(combinationMatch.GetAbilityList());
        }
        
        private void InitializeSkill(List<SkillData> skillDataList)
        {
            for (int i = skillPrefabList.Count; i < skillDataList.Count; i++)
            {
                var prefab = Instantiate(skillPrefab, skillRoot);
                skillPrefabList.Add(prefab);
            }

            for (int i = 0; i < skillPrefabList.Count; i++)
            {
                var prefab = skillPrefabList[i];
                if (i >= skillDataList.Count)
                {
                    prefab.gameObject.SetActive(false);
                    continue;
                }
                prefab.gameObject.SetActive(true);
                prefab.SetSkill(skillDataList[i]);
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Set Character Variable Reference")]
        private void SetCharacterVariableReference()
        {
            if (characterIconList == null || characterIconList.Count == 0)
                characterIconList = GetComponentsInChildren<CombinationCharacterVariableIcon>().ToList();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}