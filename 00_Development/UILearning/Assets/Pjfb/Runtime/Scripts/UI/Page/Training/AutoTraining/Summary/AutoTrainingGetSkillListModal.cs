using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class AutoTrainingGetSkillListModal : ModalWindow
    {
        [SerializeField]
        private ScrollGrid scrollGrid = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            
            TrainingAutoResultStatus result = (TrainingAutoResultStatus)args;
            // スキルリスト
            List<SkillData> skillList = new List<SkillData>();
            // 習得スキルを取得
            foreach(TrainingAbility id in result.abilityList)
            {
                skillList.Add(new SkillData(id.id, id.level));
            }
            
            // スクロールに登録
            scrollGrid.SetItems(skillList);
            
            return base.OnPreOpen(args, token);
        }
    }
}