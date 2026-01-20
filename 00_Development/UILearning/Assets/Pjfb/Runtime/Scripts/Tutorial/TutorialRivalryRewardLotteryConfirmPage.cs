using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Rivalry;

namespace Pjfb
{
    public class TutorialRivalryRewardLotteryConfirmPage : RivalryRewardLotteryConfirmPage
    {
        protected override UniTask Init()
        {
            isTutorial = true;
            return base.Init();
        }

        protected override void OnFinishedInit()
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }
    }
}