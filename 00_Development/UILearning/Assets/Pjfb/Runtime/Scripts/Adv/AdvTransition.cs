using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

using CruFramework.Adv;
using Cysharp.Threading.Tasks;

namespace Pjfb.Adv
{
    public class AdvTransition : MonoBehaviour
    {
    
        public enum FadeState
        {
            None,
            FadeOut, FadeIn
        }
    
        private const string OpenAnimation = "Open";
        private const string CloseAnimation = "Close";
    
        [SerializeField]
        private Animator transitionAnimator = null;
        [SerializeField]
        private Animator passingAnimator = null;

        private FadeState state = FadeState.None;
        /// <summary>ステート</summary>
        public FadeState State{get{return state;}}

        public UniTask FadeOut()
        {
            state = FadeState.FadeOut;
            return AnimatorUtility.WaitStateAsync(transitionAnimator, OpenAnimation);
        }
        
        public  UniTask FadeIn()
        {
            state = FadeState.FadeIn;
            return AnimatorUtility.WaitStateAsync(transitionAnimator, CloseAnimation);
        }
        
        public  UniTask PlayPassingOfTime()
        {
            return AnimatorUtility.WaitStateAsync(passingAnimator, OpenAnimation);
        }

    }
}