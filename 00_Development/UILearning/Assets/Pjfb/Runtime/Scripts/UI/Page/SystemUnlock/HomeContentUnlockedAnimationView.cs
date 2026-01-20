using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.SystemUnlock
{
    public class HomeContentUnlockedAnimationView : MonoBehaviour
    {
        public enum AnimationKeys
        {
            Default,
            Open,
            Idle,
            Close,
        }
        
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private Animator animator;
        private CancellationToken token;

        public void SetTextLabel(string labelText)
        {
            textLabel.text = labelText;
        }

        public async UniTask PlayAnimationAsync(CancellationToken cancellationToken)
        {
            token = cancellationToken;
            
            await OpenAnimation();

            await WaitCloseAsync();
        }

        public void OnClickCloseButton()
        {
            ClickCloseButton().Forget();
        }

        private async UniTask ClickCloseButton()
        {
            await AnimatorUtility.WaitStateAsync(animator, AnimationKeys.Close.ToString(), token: token);
        }

        private async UniTask OpenAnimation()
        {
            await AnimatorUtility.WaitStateAsync(animator, AnimationKeys.Open.ToString(), token: token);
        }

        private async UniTask WaitCloseAsync()
        {
            await UniTask.WaitUntil(
                () => animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationKeys.Close.ToString()),
                cancellationToken: token);
        }
    }
}