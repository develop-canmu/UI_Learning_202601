using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using CruFramework.Adv;

namespace Pjfb.Adv
{
    public class AdvMessageLogModal : ModalWindow
    {
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            List<AdvMessageLogData> logData = (List<AdvMessageLogData>)args;
            scrollGrid.SetItems(logData);
            scrollGrid.verticalNormalizedPosition = 0;
            
            return base.OnPreOpen(args, token);
        }
    }
}
