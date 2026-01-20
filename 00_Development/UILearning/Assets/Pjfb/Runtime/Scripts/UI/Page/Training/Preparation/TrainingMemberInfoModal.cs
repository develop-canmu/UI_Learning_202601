using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingMemberInfoModal : ModalWindow
    {
        [SerializeField]
        private TrainingMemberView memberView = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingMainArguments arguments = (TrainingMainArguments)args;
            // MCharId
            memberView.SetTrainingCharacter(arguments.Pending.mCharaId, arguments.TrainingCharacter.Lv, arguments.TrainingCharacter.LiberationId, arguments.Pending.mTrainingScenarioId, arguments.Pending.supportDetailList);
            
            return base.OnPreOpen(args, token);
        }
    }
}