using Pjfb.MatchResult;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;

namespace Pjfb
{
    public class TutorialRivalryRewardStealCharaPage : RivalryRewardStealCharaPage
    {

        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickLottery).Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        private new void OnClickLottery()
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            var m = (TutorialRivalryPage)Manager;

            var choicePrizeContainer = AppManager.Instance.TutorialManager.GetHuntChoicePrizeContainer();
            var mCharaId = choicePrizeContainer.choiceCharaId;
            prizeSetList = choicePrizeContainer.prizeSet;
            
            
            m.InitRewardTransition(mCharaId, NextTransition, prizeSetList);
            m.OpenRewardTransition();
        }
        
        private void NextTransition()
        {
            var nextPageData = new RivalryRewardLotteryConfirmPage.Data();
            nextPageData.prizeSetList = prizeSetList;
            nextPageData.mHuntEnemyId = data.huntFinishResponse.mHuntEnemyId;
            nextPageData.mHuntTimetableId = data.huntFinishResponse.mHuntTimetableId;
            var m = (TutorialRivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryRewardLotteryConfirm, true, nextPageData);
        }
    }
}