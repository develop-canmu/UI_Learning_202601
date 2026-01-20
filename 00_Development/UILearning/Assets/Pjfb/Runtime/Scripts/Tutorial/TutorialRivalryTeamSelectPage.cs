using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;

namespace Pjfb
{
    public class TutorialRivalryTeamSelectPage : RivalryTeamSelectPage
    {
        protected override void OnOpened(object args)
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            base.OnOpened(args);
        }
        
        protected override void OnClickPoolListItem(RivalryTeamSelectPoolListItem.ItemParams itemParams)
        {
            var param = new TeamConfirmPage.PageParams(PageType.TutorialRivalry, null, itemParams.huntMasterObject, itemParams.huntTimetableMasterObject, itemParams.huntEnemyMasterObject);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TutorialTeamConfirm, true, param);
        }
        
        protected override async UniTask GetHuntGetTimetableDetailAPI(long mHuntTimetableId)
        {
            _response = AppManager.Instance.TutorialManager.GetHuntGetTimetableContainer().huntGetTimetableDetail;
            await new UniTask();
        }
    }
}