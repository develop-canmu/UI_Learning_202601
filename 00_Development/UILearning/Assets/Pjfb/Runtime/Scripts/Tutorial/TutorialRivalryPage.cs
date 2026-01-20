using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Rivalry;

namespace Pjfb
{
    public class TutorialRivalryPage : RivalryPage
    {
        protected override string GetAddress(RivalryPageType page)
        {
            return $"Prefabs/UI/Page/TutorialRivalry/{page}Page.prefab";
        }
        
        /// <summary>事前準備</summary>
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = new Data();
            if (args != null)
            {
                data.pageType = RivalryPageType.RivalryTop;
            }
            else
            {
                data.pageType = RivalryPageType.RivalryRewardStealChara;
                data.showHeaderAndFooter = false;
                Rivalry.RivalryRewardStealCharaPage.Data postData = new Rivalry.RivalryRewardStealCharaPage.Data();
                postData.huntFinishResponse = AppManager.Instance.TutorialManager.GetHuntFinishContainer().huntFinishApiResponse;
                data.args = postData;
            }
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialRivalry);
            return OpenPageAsync(data.pageType, true, data.args, token);
        }

        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialRivalry);
            base.OnClosed();
        }
    }
}