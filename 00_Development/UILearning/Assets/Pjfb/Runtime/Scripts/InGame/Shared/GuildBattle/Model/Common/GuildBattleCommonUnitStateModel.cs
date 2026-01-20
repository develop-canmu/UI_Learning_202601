using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonUnitStateModel
    {
        #region Properties and SerializeField

        [Key(0)] public int UnitId;
        [Key(1)] public int PartyId;
        [Key(2)] public int RevivalTurn;
        [Key(3)] public bool IsDeparted;

        private int BeatenCount;
        
        #endregion

        #region Fields

        #endregion

        #region Public Methods

        /*
        public void SetData(BattleUnitModel unit)
        {
            PartyId = 0;
            RevivalTurn = 0;
            UnitId = unit.Id;
        }
        */

        public bool IsDead()
        {
            return RevivalTurn > 0;
        }

        public void Depart()
        {
            IsDeparted = true;
        }
        
        public void SetPartyId(int partyId)
        {
            PartyId = partyId;
        }

        public void DecrementRevivalTurn()
        {
            RevivalTurn--;
        }

        public void OnDeadUnit(int revivalTurn)
        {
            RevivalTurn = revivalTurn;
            IsDeparted = false;
        }

        /// <summary>
        /// 解散フロー
        /// </summary>
        public void OnDissolutionParty()
        {
            PartyId = 0;
            IsDeparted = false;
        }

        /// <summary>
        /// 再出発から個別で外されたフロー. PTから追放された件
        /// </summary>
        public void OnRemovedFromParty()
        {
            IsDeparted = false;
            PartyId = 0;
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}