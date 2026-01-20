using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using CruFramework;
using Pjfb.Master;
using System;
using System.Text.RegularExpressions;
using Pjfb.UserData;

using CruFramework.UI;
using System.Linq;

namespace Pjfb.Training
{
    public class TrainingScenarioSpecialAttackModal : ModalWindow
    {
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        private long scenarioId = 0;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            scenarioId = (long)args;
            
            List<TrainingPracticeSkillView.ViewData> skillList = new List<TrainingPracticeSkillView.ViewData>();
            
            Dictionary<long, CharaTrainingStatusMasterObject> mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindLvMaxData(scenarioId);
            
            // 所持キャラ
            foreach(KeyValuePair<long, CharaTrainingStatusMasterObject> status in mStatus)
            {
                // スキルをリストに追加
                List<PracticeSkillInfo> list = PracticeSkillUtility.GetCharacterPracticeSkill(status.Value.mCharaId,
                    status.Value.level, status.Value.mTrainingScenarioId, false);

                for(int i=0;i<list.Count;i++)
                {
                    var charaTrainingStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(list[i].MasterId);
                    TrainingPracticeSkillView.ViewData viewData = new TrainingPracticeSkillView.ViewData(list[i], false, charaTrainingStatus.mCharaId, -1, i, false, -1, -1, null, false);
                    skillList.Add(viewData);
                }                
            }
            
            // キャラId順にソート
            TrainingPracticeSkillView.ViewData[] sortedList = skillList.OrderBy((v) =>
            {
                var charaTrainingStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(v.SkillInfo.MasterId);
                return charaTrainingStatus.mCharaId;
            }).ToArray();
            
            // スクロールにセット
            scrollGrid.SetItems(sortedList);
            await base.OnPreOpen(args, token);
        }
    }
}