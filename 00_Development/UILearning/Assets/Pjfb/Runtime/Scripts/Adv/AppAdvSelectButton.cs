using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb
{
    public class AppAdvSelectButton : AdvSelectButton
    {
    
        private static readonly string SelectedAnimation = "Confirmed";
        private static readonly string DisappearAnimation = "Disappear";
        
        [SerializeField]
        private RubyTextMeshProUGUI text = null;
        
        [SerializeField]
        private Animator animator = null;
        
        public UniTask PlaySelectAnimation()
        {
            Button.interactable = false;
            return AnimatorUtility.WaitStateAsync(animator, SelectedAnimation);
        }
        
        public UniTask PlayDisappearAnimation()
        {
            Button.interactable = false;
            return AnimatorUtility.WaitStateAsync(animator, DisappearAnimation);
        }

        public override void SetMessage(string message)
        {
            Button.interactable = true;
            text.UnditedText = message;
        }
    }
}