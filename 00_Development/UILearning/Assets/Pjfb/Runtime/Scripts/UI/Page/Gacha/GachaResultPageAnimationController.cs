using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Master;
using Pjfb.UserData;


namespace Pjfb.Gacha
{
    public class GachaResultPageAnimationController : MonoBehaviour
    {
        readonly string CloseResultTrigger = "CloseResult";
        readonly string CloseRedrawChanceTrigger = "CloseRedrawChance";
        readonly string BackToResultTrigger = "BackToResult";
        readonly string OpenRedrawChanceTrigger = "OpenRedrawChance";
        readonly string IdleResultTrigger = "IdleResult";
        readonly string IdleRedrawChanceTrigger = "IdleRedrawChance";
        
        
        

        [SerializeField]
        Animator resultPageAnimator = null;

        public void OpenResultPage(){
            resultPageAnimator.SetTrigger(IdleResultTrigger);
        }

        public void OpenPendingPage(){
            resultPageAnimator.SetTrigger(IdleRedrawChanceTrigger);
        }

        public async UniTask PlayInResultPage(){
            await AnimatorUtility.WaitStateAsync(resultPageAnimator, BackToResultTrigger);
        }

        public async UniTask PlayOutResultPage(){
            await AnimatorUtility.WaitStateAsync(resultPageAnimator, CloseResultTrigger);
        }

        public async UniTask PlayInPendingPage(){
            await AnimatorUtility.WaitStateAsync(resultPageAnimator, OpenRedrawChanceTrigger);
        }

        public async UniTask PlayOutPendingPage(){
            await AnimatorUtility.WaitStateAsync(resultPageAnimator, CloseRedrawChanceTrigger);
        }

        

    }
}
