using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class AcquisitionOfLotteryRightsAnimation : MonoBehaviour
    {
        private const string OpenTrigger = "Open";
        private const string MoveIconTrigger = "MoveIcon";
        private const string CloseTrigger = "Close";

        [SerializeField] private Animator animator = null;
        [SerializeField] private GameObject touchGuard = null;
        [SerializeField] private Image textAcquisitionImage;
        [SerializeField] private List<Sprite> textAcquisitionSprites;

        public bool IsPlayingCloseAnimation { get; private set; } = false;

        public async UniTask PlayOpenEffectAsync(long effectType, Action onComplete)
        {
            touchGuard.SetActive(true);
            gameObject.SetActive(true);

            textAcquisitionImage.sprite = textAcquisitionSprites[(int)effectType];
            
            await AnimatorUtility.WaitStateAsync(animator, OpenTrigger);
            touchGuard.SetActive(false);
            onComplete();
        }

        public async UniTask PlayMoveIconEffectAsync(Action onComplete)
        {
            touchGuard.SetActive(true);
            gameObject.SetActive(true);

            await AnimatorUtility.WaitStateAsync(animator, MoveIconTrigger);
            touchGuard.SetActive(false);
            onComplete();
        }

        public async UniTask PlayCloseEffectAsync(Action onCompleted)
        {
            if (IsPlayingCloseAnimation) return;

            IsPlayingCloseAnimation = true;
            
            await AnimatorUtility.WaitStateAsync(animator, CloseTrigger);
            
            gameObject.SetActive(false);
            IsPlayingCloseAnimation = false;
            onCompleted();
        }
    }
}
