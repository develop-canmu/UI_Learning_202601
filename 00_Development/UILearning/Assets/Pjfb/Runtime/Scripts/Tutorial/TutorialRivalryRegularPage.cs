using Cysharp.Threading.Tasks;
using Pjfb.Rivalry;

namespace Pjfb
{
    public class TutorialRivalryRegularPage : RivalryRegularPage
    {
        protected override void OnOpened(object args)
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            base.OnOpened(args);
        }
    }
}