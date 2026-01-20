using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb
{
    public class TutorialTrainingResultPage : TrainingResultPage
    {
        protected override void OnEndOpenAnimation()
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnRankButton).Forget();
        }

        protected override void OnEndOpenRankAnimation()
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnRankNextButton).Forget();
        }
        
    }
}