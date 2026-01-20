using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Master;
using DG.Tweening;


namespace Pjfb.Gacha
{
    public class GachaPrizeIconAnimationController : MonoBehaviour
    {
        readonly string animatorKeyDisable = "Disabled";
        readonly string animatorKeyNormal = "Normal";
        
        [SerializeField]
        private Animator _animator = null;
        [SerializeField]
        private Animator[] _rarityAnimator = null;
        
        void Start(){
            //GachaPrizeIconのアニメータで色を変えるため、レアリティのアニメータをOffにする
            foreach( var animator in _rarityAnimator ) {
                animator.enabled = false;
            }
            
        }
        public void SetEnable(){
            _animator.SetTrigger(animatorKeyNormal);   
        }

        public void SetDisable(){
            _animator.SetTrigger(animatorKeyDisable);   
        }

    }
}