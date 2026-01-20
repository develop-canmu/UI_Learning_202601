using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using UnityEngine;

namespace Pjfb
{
    public class TutorialTrainingPracticeGamePreparationPage : TrainingPracticeGamePreparationPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }
        
    }
}