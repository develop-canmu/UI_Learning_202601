using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Master;
using Pjfb.UserData;


namespace Pjfb.Gacha
{
    public class GachaRetryAnimationController : MonoBehaviour
    {

        readonly string OpenTrigger = "Open";
        readonly string CloseTrigger = "Close";
        [SerializeField]
        private Animator _retryAnimator = null;
        [SerializeField]
        private Image _ballImage = null;

        [SerializeField]
        private Sprite _goldBallSprite = null;

        [SerializeField]
        private Sprite _normalBallSprite = null;

        public async UniTask PlayOpenAnimation( long rarity ){
            gameObject.SetActive(true);
            var ballSprite = GachaUtility.IsGoldBallRarity(rarity) ? _goldBallSprite : _normalBallSprite;
            _ballImage.sprite = ballSprite;
            await AnimatorUtility.WaitStateAsync(_retryAnimator, OpenTrigger);
        }

        public async UniTask PlayCloseAnimation(){
            await AnimatorUtility.WaitStateAsync(_retryAnimator, CloseTrigger);
        }
        

    }
}
