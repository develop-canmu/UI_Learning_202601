using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CombinationPracticeSkillView : MonoBehaviour
    {
        [SerializeField] private Transform skillRoot;
        [SerializeField] private PracticeSkillViewMini skillPrefab ;
        [SerializeField] private GameObject lockObject;
        [SerializeField] private TMPro.TMP_Text lockText;
        
        private List<PracticeSkillViewMini> skillPrefabList = new();
        
        public void InitializeSkill(List<PracticeSkillInfo> skillDataList, bool isLock, string lockString, bool showHighlight)
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
                prefab.SetSkillData(skillDataList[i], showHighlight);
            }
            
            lockObject.SetActive(isLock);
            lockText.text = lockString;
        }
    }
}