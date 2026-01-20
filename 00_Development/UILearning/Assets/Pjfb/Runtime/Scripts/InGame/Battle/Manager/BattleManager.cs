
using Pjfb.InGame;


// ReSharper disable once CheckNamespace
namespace Pjfb.Battle
{
    public class BattleManager
    {
        private static BattleManager instance;

        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }

                return instance;
            }
            
            private set => instance = value;
        }
        public BattleBase Battle { get; private set; }
        
        public void Initialize(BattleBase battle)
        {
            Battle = battle;
        }

        public void Release()
        {
            instance = null;
        }

        public BattleBase.BattleState CurrentState => Battle?.CurrentState ?? BattleBase.BattleState.None;
        
        #region Battle

        public void StartBattle()
        {
            // リプレイのときは結果計算済みなのでログにまかせて中身の処理はしない.
            if (BattleDataMediator.Instance.IsReplayMode)
            {
                // TODO 確認 BattleEventDispatcherのイベントをSimplePage退出時にはずすべきか.
                BattleEventDispatcher.Instance.OnAddLogCallback();
                return;
            }
            
            Battle.StateAction(BattleBase.BattleState.StartBattle);
        }

        public void DoAutoResolveMatchUp()
        {
            Battle.DoAutoResolveMatchUp();
        }

        public void JudgeMatchUpFinalResult(bool isSwiped)
        {
            Battle.JudgeMatchUpFinalResult(isSwiped);
        }

        public void DoNextMatchUp()
        {
            if (Battle == null)
            {
                return;
            }
            var nextState = Battle.GetNextState(BattleDataMediator.Instance.NextMatchUpResult);
            Battle.StateAction(nextState);
        }

        #endregion
    }
}