using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Voice;
using TMPro;
using Pjfb.Battle;

namespace Pjfb.InGame
{
    public class InGameSkipModal : ModalWindow
    {
        [SerializeField] private TextMeshProUGUI mainText;

        // TODO 一旦許して.
        private const string SkipToEndMessage = "試合をスキップしますか?";

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            mainText.text = SkipToEndMessage;
            return base.OnPreOpen(args, token);
        }

        public void OnClickYes()
        {
            // failsafe
            var currentPage = (NewInGamePage)AppManager.Instance.UIManager.PageManager.CurrentPageObject;
            if (currentPage == null || currentPage.CurrentPageType != NewInGamePageType.SimplePage)
            {
                Close();
                return;
            }
	    
            Close();

            // 連打ブロック && 処理中ごまかし
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            AppManager.Instance.UIManager.System.Loading.Show();
            
            BattleDataMediator.Instance.IsSkipToFinish = true;
            SkipToFinish();
        }

        private void SkipToFinish()
        {
            if (BattleDataMediator.Instance.IsReplayGameMode)
            {
                BattleEventDispatcher.Instance.OnBattleEndCallback();
                return;
            }

            if (BattleManager.Instance.CurrentState == BattleBase.BattleState.SelectMatchUpAction)
            {
                BattleManager.Instance.DoAutoResolveMatchUp();
            }
        }

        public void OnClickNo()
        {
            Close();
        }
    }
}