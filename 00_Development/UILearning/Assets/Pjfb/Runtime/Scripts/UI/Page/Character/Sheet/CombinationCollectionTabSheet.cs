using System.Collections.Generic;
using System.Linq;
using CruFramework.Page;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Character
{
    public class CombinationCollectionTabSheet : Sheet
    {
        [SerializeField] private CombinationCollectionScrollDynamic scrollDynamic;

        private IReadOnlyList<CombinationOpenedMinimum> latestDataList;
        
        public void Initialize(IReadOnlyList<CombinationOpenedMinimum> dataList)
        {
            latestDataList = dataList;
            scrollDynamic.InitializeSelectOpend(latestDataList);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClickPracticeSkillListButton()
        {
            var combinationTrainingStatusIds = latestDataList
                .Select(item =>
                {
                    var combinationTrainingStatus = MasterManager.Instance.combinationTrainingStatusMaster.FindDataByCombinationIdAndProgressLevel(item.mCombinationId, item.progressLevel);
                    return combinationTrainingStatus.id;
                })
                .ToArray();
            
            var totalPracticeSkillList =
                PracticeSkillUtility.GetCombinationTotalPracticeSkill(combinationTrainingStatusIds);
            var data = new CombinationCollectionPracticeSkillListModal.Data(totalPracticeSkillList);
            CombinationCollectionPracticeSkillListModal.Open(data);
        }
    }
}