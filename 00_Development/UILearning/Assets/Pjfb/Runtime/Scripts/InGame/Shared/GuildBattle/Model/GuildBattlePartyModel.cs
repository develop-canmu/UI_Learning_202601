using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using MessagePack;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    [MessagePackObject]
    public class GuildBattlePartyModel : GuildBattleCommonPartyModel
    {
        [Key(31)] public int Identifier; //まじで名前がよくないから考え直す.
        [Key(32)] public long RevivalTurn;
        [Key(33)] public List<long> UCharaIds = new List<long>(5);
        [Key(34)] public int StartBallCount;
        [Key(35)] public string DeckName;

        [IgnoreMember] public bool LocalUpdateFlag = false; // ClientOnly. SendやDissolve通信してから次回更新まではTrue.

        public void SetData(long playerId, int partyId, int playerPartyId, SendPartyCommand command, /*BattleTacticsModel tactics,*/ GuildBattleCommonConst.GuildBattleTeamSide side, GuildBattleCommonMapSpotModel startSpot, GuildBattleCommonMapSpotModel targetSpot)
        {
            State = PartyState.OnMap;
            PlayerId = playerId;
            PlayerPartyId = playerPartyId;
            PartyId = partyId;
            PlayerIndex = command.PlayerIndex;
            UnitIds = command.GroupUnitIdList.ToList();
            
            StartSpotId = startSpot.Id;
            LastJoinedSpotId = startSpot.Id;
            TargetSpotId = targetSpot.Id;
            LaneNumber = targetSpot.LaneNumber;
            XPosition = startSpot.PositionX;
            LastXPosition = startSpot.PositionX;
            Side = side;
            TacticsId = command.TacticsId;
            RoleOperationIds = command.RoleOperationIds;
            WinStreakCount = 0;

            MilitaryStrength = new List<int>(1) { command.TotalMilitaryStrength };
            MaxMilitaryStrength = new List<int>(1) { command.TotalMilitaryStrength };
            LastMilitaryStrength = new List<int>(1) { command.TotalMilitaryStrength };
            StartBallCount = command.TotalMilitaryStrength;
        }

        public BattleV2Chara GetLeaderCharacterData()
        {
            var leaderUCharaId = UCharaIds.FirstOrDefault();
            return PjfbGuildBattleDataMediator.Instance.BattleCharaData[leaderUCharaId];
        }

        public void OnWonFight(int damage)
        {
            MilitaryStrength[0] = Math.Max(MilitaryStrength[0] - damage, 0);
            if (StartBallCount > GetBallCount())
            {
                MilitaryStrength[0]++;
            }
        }
        
        public int GetBallCount()
        {
            return MilitaryStrength[0];
        }

        public int GetLastBallCount()
        {
            return LastMilitaryStrength[0];
        }
        
        public bool IsDead()
        {
            return RevivalTurn > 0;
        }

        public bool IsValid()
        {
            return UCharaIds.Count == 5;
        }
        
        public override void OnDeadParty()
        {
            var player = GuildBattleCommonDataMediator.Instance.GetBattlePlayerData(PlayerIndex);
            RevivalTurn = player.GetRevivalTurnCount();
            
            base.OnDeadParty();
        }

        public override void OnDissolutionParty(bool isBeforeFight)
        {
            // 開戦後 && マップ上にいる場合のみペナルティをかける.
            // マップ上にいない場合=戦闘のフローでOnDeadPartyにいっている.
            if (!isBeforeFight && IsOnMap())
            {
                OnDeadParty();
            }
            
            SetAsStandby();
        }
        
        public int GetRevivalTurnCount()
        {
            if (IsOnMap())
            {
                return -1;
            }
            
            return (int)RevivalTurn;
        }
        
        public bool IsCoolTime()
        {
            return GetRevivalTurnCount() > 0;
        }

        public (bool, string) EvaluatePartyCondition()
        {
            // TODO Stringutility
            var message = string.Empty;
            var ret = true;

            if ((BattleConst.DeckStrategy)TacticsId is not
                (BattleConst.DeckStrategy.Aggressive or BattleConst.DeckStrategy.Dribble or BattleConst.DeckStrategy.Pass))
            {
                ret = false;
#if !MAGIC_ONION_SERVER
                message = "戦術設定が無効です";
#endif
            }

            if (RoleOperationIds.Any(id => id == (long)RoleNumber.None))
            {
                ret = false;
#if !MAGIC_ONION_SERVER
                message = "無効なポジションが含まれています";
#endif
            }

            if (!(RoleOperationIds.Any(id => id == (long)RoleNumber.FW) &&
                RoleOperationIds.Any(id => id == (long)RoleNumber.MF) &&
                RoleOperationIds.Any(id => id == (long)RoleNumber.DF)))
            {
                ret = false;
#if !MAGIC_ONION_SERVER
                message = "各ポジションが最低一人は必要です";
#endif
            }
            
            return (ret, message);
        }
    }
}