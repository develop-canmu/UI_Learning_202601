using System;
using System.Collections.Generic;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonPlayerData
    {
        #region Properties and SerializeField

        [Key(0)]
        public long PlayerId;

        [Key(1)]
        public int PlayerIndex;
        
        [Key(2)]
        public long MaxMilitaryStrength;

        [Key(3)]
        public long AvailableMilitaryStrength;

        [Key(4)]
        public List<GuildBattleCommonItemData> GuildBattleItemList;

        [Key(5)]
        public GuildBattleCommonConst.GuildBattleTeamSide Side = GuildBattleCommonConst.GuildBattleTeamSide.All;
        
        [Key(6)]
        public List<GuildBattleAbilityData> GuildBattleActivatedAbilityList;

        [IgnoreMember]
        public bool IsJoinedFirstTime = true;

        [IgnoreMember]
        public int OnDeadPartyCount = 0;
        
        #endregion

        #region Fields
        #endregion

        #region Public Methods

        public int GetRevivalTurnCount()
        {
            var setting = GuildBattleCommonDataMediator.Instance.GuildBattleSetting;
            return setting.GuildBattleRevivalTurn + setting.GuildBattleRevivalTurnPenaltyPerBeaten * OnDeadPartyCount;
        }
        
        public virtual void AddAvailableMilitaryStrength(int value, bool isAllowOverheal)
        {
            if (isAllowOverheal)
            {
                AvailableMilitaryStrength += value;
            }
            else
            {
                AvailableMilitaryStrength = Math.Clamp(AvailableMilitaryStrength + value, 0, MaxMilitaryStrength);
            }
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}