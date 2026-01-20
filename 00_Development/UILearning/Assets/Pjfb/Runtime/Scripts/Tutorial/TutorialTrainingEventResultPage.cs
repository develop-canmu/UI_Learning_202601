using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb
{
    public class TutorialTrainingEventResultPage : TrainingEventResultPage
    {

        private Action OnSkillClosed;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            OnSkillClosed = () =>
            {
                SkillClosedActionAsync().Forget();
            };
            await base.OnPreOpen(args, token);
        }

        protected override void NextEffect()
        {
            if (animationState == AnimationState.SkillClose &&
                OnSkillClosed != null)
            {
                OnSkillClosed();
                OnSkillClosed = null;
                return;
            }
            base.NextEffect();
        }

        private async UniTask SkillClosedActionAsync()
        {
            AppManager.Instance.TutorialManager.ShowTouchGuard();
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction();
            AppManager.Instance.TutorialManager.HideTouchGuard();
        }
    }
}