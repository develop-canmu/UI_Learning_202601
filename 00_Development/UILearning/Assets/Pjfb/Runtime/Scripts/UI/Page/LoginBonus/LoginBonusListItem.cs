using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.LoginBonus
{
    public class LoginBonusListItem : MonoBehaviour {
        
        [SerializeField] 
        private PrizeJsonView prizeJsonView;
        [SerializeField] 
        private Animator StampAnimator;

        public long day = -1;

        public void Init(PrizeJsonWrap prize,long day,bool isActive)
        {
            prizeJsonView.SetView(prize);
            this.day = day;
            SetStampImageActive(isActive);
        }

        private async void SetStampImageActive( bool isActive )
        {
            //activeとinactive後、animator反映の為1 frame待ち
            await UniTask.NextFrame();
            if (isActive)
            {
                //Stamp表示
                StampAnimator.SetTrigger("Got");
            }
            else
            {
                //Stamp非表示
                StampAnimator.Play("Null", 0, 1f);
            }
        }

        public void OpenStamp(bool toEnd = false)
        {
            if (toEnd)
            {
                StampAnimator?.SetTrigger("Got");
            }
            else
            {
                //Stamp獲得演出
                StampAnimator?.SetTrigger("Stamp");
            }
        }

    }
}