using System.Collections.Generic;
using MessagePack;
using Pjfb.Networking.App.Request;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonResultData
    {
        #region Properties and SerializeField

        [Key(0)]
        public GuildBattleCommonConst.GuildBattleTeamSide WinTeam = GuildBattleCommonConst.GuildBattleTeamSide.All;

        [Key(1)]
        public Dictionary<long, int> SpotDefenceCount;
        [Key(2)]
        public Dictionary<long, int> WinFightCount;
        [Key(3)]
        public Dictionary<long, int> MaxWinStreakCount;
        [Key(4)]
        public Dictionary<long, int> TotalDamageCount;

        #endregion

        #region Fields

        #endregion

        #region Public Methods

        public GuildBattleCommonResultData()
        {
        }

        public void UpdateSpotDefenceCount(GuildBattleCommonPartyModel party)
        {
            if (!party.IsDefendingAtAnySpot())
            {
                return;
            }
            
            SpotDefenceCount[party.PlayerId]++;
        }

        public void UpdateWinFightCount(GuildBattleCommonPartyModel party)
        {
            WinFightCount[party.PlayerId]++;
        }
        
        public void UpdateMaxWinStreakCount(GuildBattleCommonPartyModel party)
        {
            if (MaxWinStreakCount[party.PlayerId] < party.WinStreakCount)
            {
                MaxWinStreakCount[party.PlayerId] = party.WinStreakCount;
            }
        }
        
        public void UpdateTotalDamageCount(GuildBattleCommonPartyModel party, int dealtDamage)
        {
            TotalDamageCount[party.PlayerId] += dealtDamage;
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}