using Cysharp.Threading.Tasks;
namespace Pjfb
{
    public class TutorialTeamConfirmPage : TeamConfirmPage
    {
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }

    }
}