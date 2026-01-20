using Pjfb.MatchResult;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TutorialMatchResultWinPage : MatchResultWinPage
    {
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickNext).Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }

        private new void OnClickNext()
        {
            AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.TutorialRivalry, false, null).Forget(); 
        }
    }
}