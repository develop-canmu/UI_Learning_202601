using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingMenuDetailTargetModal : ModalWindow
    {
        [SerializeField]
        private TMP_Text targetText = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            long mTrainingScenarioId = (long)args;
            TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(mTrainingScenarioId);
            targetText.text = mTrainingScenario.goalDescription;
            return base.OnPreOpen(args, token);
        }
    }
}