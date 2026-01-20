using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TrainingDeckEnhanceDetailModal : ModalWindow
    {
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // todo: 仮で作成
            return base.OnPreOpen(args, token);
        }
    }
}