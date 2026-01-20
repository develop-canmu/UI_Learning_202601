using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Battle;

namespace Pjfb.InGame
{
    public class LambdaBattle : BattleBase
    {
        public void AddManagers()
        {
            var battleDataMediator = new BattleDataMediator();
            var battleLogMediator = new BattleLogMediator();
            var battleEventDispatcher = new BattleEventDispatcher();

            BattleDataMediator.Instance.BattleType = BattleConst.BattleType.ExecLeagueMatch;
            BattleDataMediator.Instance.IsSkipToFinish = true;
            BattleDataMediator.Instance.IsSkipToFinishWithoutView = true;
            battleEventDispatcher.OnAddLogAction = OnAddLog;
            battleEventDispatcher.OnActivateActiveAbilityAction = OnActivateActiveAbility;
        }

        public void ResetManagers()
        {
            BattleLogMediator.Instance.ClearLogs();
            // BattleDataMediatorはデータセット時にどうせ初期化するため不要.
            // BattleEventDispatcherは…まあ初期化する必要性もないので.
        }
        
        public override void StateAction(BattleState state)
        {
            base.StateAction(state);
        }

        private void OnAddLog()
        {
            var log = BattleLogMediator.Instance.BattleLogs.LastOrDefault();
            switch (log.LogTiming)
            {
                case BattleConst.LogTiming.DoNextMatchUp:
                    DoNextMatchUp();
                    break;
                case BattleConst.LogTiming.OnBattleEnd:
                    OnBattleEnd();
                    break;
            }
        }
        
        private void OnActivateActiveAbility()
        {
            BattleLogMediator.Instance.AddActiveAbilityLog(BattleDataMediator.Instance.NextMatchUpResult);
        }


        private void DoNextMatchUp()
        {
            var nextState = GetNextState(BattleDataMediator.Instance.NextMatchUpResult);
            StateAction(nextState);
        }

        private void OnBattleEnd()
        {
        }
    }
}