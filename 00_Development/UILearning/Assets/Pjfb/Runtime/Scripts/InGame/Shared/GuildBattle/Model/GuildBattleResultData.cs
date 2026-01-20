using System.Collections.Generic;
using MagicOnion;
using MessagePack;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    [MessagePackObject]
    public class GuildBattleResultData : GuildBattleCommonResultData {
        public GuildBattleResultData()
        {
            
        }
        
        public GuildBattleResultData(BattleV2ClientData battleData)
        {
            var playerCount = battleData.playerList.Length;
            SpotDefenceCount = new Dictionary<long, int>(playerCount);
            WinFightCount = new Dictionary<long, int>(playerCount);
            MaxWinStreakCount = new Dictionary<long, int>(playerCount);
            TotalDamageCount = new Dictionary<long, int>(playerCount);

            foreach (var player in battleData.playerList)
            {
                SpotDefenceCount.Add(player.playerId, 0);
                WinFightCount.Add(player.playerId, 0);
                MaxWinStreakCount.Add(player.playerId, 0);
                TotalDamageCount.Add(player.playerId, 0);
            }
        }
    }
}