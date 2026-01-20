using System;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.InGame;

namespace Pjfb
{
    public class TutorialNewInGameResultPage : NewInGameResultPage
    { 
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.HideTouchGuard();
            AppManager.Instance.UIManager.System.Loading.Hide();
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            return base.OnPreOpen(args, token);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
        }

        protected override async void OnClickNextButton()
        {
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction();
        }
    }
}