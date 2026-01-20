using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace MagicOnion
{
    /// <summary>
    /// MagicOnion経由でクライアントから飛んでくるコマンド群定義
    /// </summary>

    public enum ClientCommandType
    {
        None,
        SendParty,
        DissolutionParty,
    } 

    /// <summary>
    /// コマンドキューを作りやすくする & 今後の拡張性のためにBaseの定義しておく.
    /// </summary>
    [MessagePackObject]
    public class ClientCommandBase
    {
        [IgnoreMember]
        public virtual ClientCommandType GetCommandType => ClientCommandType.None;

        [IgnoreMember]
        public int PlayerIndex;
    }
    
    /// <summary>
    /// 軍隊の出撃命令
    /// </summary>
    [MessagePackObject]
    public class SendPartyCommand : ClientCommandBase
    {
        [IgnoreMember]
        public override ClientCommandType GetCommandType => ClientCommandType.SendParty;
        
        [Key(1)]
        public int TargetSpotId { get; set; }
        
        // 新規出兵用データ
        [Key(2)]
        public int TotalMilitaryStrength { get; set; }
        [Key(3)]
        public long[] GroupUnitIdList { get; set; }
        [Key(4)]
        public long TacticsId { get; set; }
        [Key(5)]
        public long[] RoleOperationIds { get; set; }

        [Key(6)]
        public int DepartedPartyId; 

        public SendPartyCommand()
        {
            GroupUnitIdList = new long[] { -1, -1, -1, -1, -1 };
            RoleOperationIds = new long[] { -1, -1, -1, -1, -1 };
        }

        /*
        public bool IsDefenceCommand()
        {
            var player = GuildBattleDataMediator.Instance.GetBattlePlayerData(PlayerIndex);
            var spot = GuildBattleDataMediator.Instance.BattleField.GetMapSpot(TargetSpotId);

            if (player == null || spot == null)
            {
                return false;
            }

            return player.Side == spot.OccupyingSide;
        }
        */
    }
    
    /// <summary>
    /// 軍隊の解散命令
    /// </summary>
    [MessagePackObject]
    public class DissolutionPartyCommand : ClientCommandBase
    {
        [IgnoreMember]
        public override ClientCommandType GetCommandType => ClientCommandType.DissolutionParty;
        
        [Key(1)]
        public int TargetPartyId { get; set; }
    }
}