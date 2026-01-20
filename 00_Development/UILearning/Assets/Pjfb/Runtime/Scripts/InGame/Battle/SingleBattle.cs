using Pjfb.InGame;
using Pjfb.Networking.App.Request;


namespace Pjfb.Battle
{
    public class SingleBattle : BattleBase
    {
        #region Imple Method

        public void SetDummyData()
        {
            BattleV2ClientData clientData = AppManager.Instance.TutorialManager.GetBattleV2ClientData(0);
            BattleDataMediator.Instance.InitializeGameData(clientData);
            // そもそもUI系がBoltServerにいるのはやばいけどClientとわけてないから一旦.
            BattleEventDispatcher.Instance.OnSetBattleDataCallback();
        }

        public override void SetData(BattleV2ClientData data)
        {
            base.SetData(data);
            
            // そもそもUI系がBoltServerにいるのはやばいけどClientとわけてないから一旦.
            BattleEventDispatcher.Instance.OnSetBattleDataCallback();
        }

        #endregion
    }
}