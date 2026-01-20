using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class SupportEquipmentPracticeSkillListView : MonoBehaviour
    {
        [SerializeField] private RectTransform practiceSkillRoot;
        [SerializeField] private PracticeSkillViewMini practiceSkillViewMiniPrefab;

        private List<PracticeSkillViewMini> practiceSkillViewMiniList = new ();

        public void InitializeUI(List<PracticeSkillInfo> practiceSkillDataList)
        {
            if(practiceSkillDataList.Count <= 0) return;
            
            if (practiceSkillViewMiniList.Count <= 0)
            {
                practiceSkillViewMiniList.Add(practiceSkillViewMiniPrefab);
            }
            
            practiceSkillViewMiniList.ForEach(possessionItem => possessionItem.gameObject.SetActive(false));
            for(var i = 0; i < practiceSkillDataList.Count; i++)
            {
                if (practiceSkillViewMiniList.Count <= i)
                {
                    var obj = Instantiate(practiceSkillViewMiniPrefab.gameObject,practiceSkillRoot);
                    var practiceSkillViewMini = obj.GetComponent<PracticeSkillViewMini>();
                    practiceSkillViewMiniList.Add(practiceSkillViewMini);
                }
                practiceSkillViewMiniList[i].SetSkillData(practiceSkillDataList[i], false, false);
                practiceSkillViewMiniList[i].gameObject.SetActive(true);
            }
        }
        
        public void InitializeUI(List<PracticeSkillLotteryInfo> practiceSkillDataList)
        {
            if(practiceSkillDataList.Count <= 0) return;
            
            if (practiceSkillViewMiniList.Count <= 0)
            {
                practiceSkillViewMiniList.Add(practiceSkillViewMiniPrefab);
            }
            
            practiceSkillViewMiniList.ForEach(possessionItem => possessionItem.gameObject.SetActive(false));
            for(var i = 0; i < practiceSkillDataList.Count; i++)
            {
                if (practiceSkillViewMiniList.Count <= i)
                {
                    var obj = Instantiate(practiceSkillViewMiniPrefab.gameObject,practiceSkillRoot);
                    var practiceSkillViewMini = obj.GetComponent<PracticeSkillViewMini>();
                    practiceSkillViewMiniList.Add(practiceSkillViewMini);
                }
                practiceSkillViewMiniList[i].SetSkillData(practiceSkillDataList[i]);
                practiceSkillViewMiniList[i].gameObject.SetActive(true);
            }

            // アイテムをセットし終わってからサイズを再計算する
            LayoutRebuilder.ForceRebuildLayoutImmediate(practiceSkillRoot);
        }
    }
}

