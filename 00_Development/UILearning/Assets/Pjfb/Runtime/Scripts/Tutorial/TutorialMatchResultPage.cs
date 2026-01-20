using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.MatchResult;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class TutorialMatchResultPage : MatchResultPage
    {
        protected override string GetAddress(MatchResultPageType page)
        {
            return $"Prefabs/UI/Page/TutorialMatchResult/{page}Page.prefab";
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();
            base.OnEnablePage(args);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = new Data();
            data.pageType = MatchResultPageType.MatchResultWin;

            var huntFinishContainer = AppManager.Instance.TutorialManager.GetHuntFinishContainer();
            var nextParam = new MatchResultWinPage.Data(
                PageType.Rivalry,
                1,
                huntFinishContainer.huntFinishApiResponse,
                huntFinishContainer.mvpCharaId
            );
            
            data.args = null;
            return OpenPageAsync(data.pageType, true, nextParam, token);
        }
        
    }
}