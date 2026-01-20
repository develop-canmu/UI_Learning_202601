using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Networking.App.Request;
using UnityEngine;
using UnityEngine.UI;

using Pjfb.Master;

namespace Pjfb.Training
{
    
    public class TrainingPracticeConfirmModal : ModalWindow
    {
    
        [SerializeField]
        private UIToggle toggle = null;
    
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            TrainingUtility.IsConfirmPracticeGameModal = toggle.isOn == false;
        }

        public void OnOkButton()
        {
            SetCloseParameter(true);
            Close();
        }
    }
}