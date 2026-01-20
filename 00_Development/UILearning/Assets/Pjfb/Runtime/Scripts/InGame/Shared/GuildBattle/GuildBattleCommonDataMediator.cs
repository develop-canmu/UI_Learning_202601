using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pjfb;

namespace MagicOnion
{
    public abstract class GuildBattleCommonDataMediator
    {
        public static GuildBattleCommonDataMediator Instance { get; protected set; }

        public IGuildBattleSetting GuildBattleSetting { get; protected set; }
        
        public GuildBattleCommonFieldModel BattleField { get; protected set; }
        public Dictionary<long, GuildBattleCommonPlayerData> BattlePlayerDataById { get; protected set; } = new Dictionary<long, GuildBattleCommonPlayerData>(); // PlayerId, PlayerData
        public Dictionary<long, GuildBattleCommonPlayerData> BattlePlayerDataByIndex { get; protected set; } = new Dictionary<long, GuildBattleCommonPlayerData>(); // PlayerIndex, PlayerData
        public SortedDictionary<int, GuildBattleCommonPartyModel> BattleParties { get; protected set; } = new SortedDictionary<int, GuildBattleCommonPartyModel>(); // PartyId, Party
        public Dictionary<long, GuildBattleUnitStateModel> StandbyUnitDictionary { get; private set; } = new Dictionary<long, GuildBattleUnitStateModel>(); // UnitId, StandbyUnit
        public Dictionary<long, List<GuildBattleUnitStateModel>> StandbyUnits { get; private set; } = new Dictionary<long, List<GuildBattleUnitStateModel>>(); // PlayerIndex, List<StandbyUnit>

        public GuildBattleCommonFieldSituationModel LatestSituation { get; set; }
        public GuildBattleCommonResultData ResultData { get; protected set; }
        
        public readonly int[] WinningPoints = new int[] { 0, 0 };
        // 勝利点計算用.
        public readonly int[] KillMilitaryStrength = new int[] { 0, 0 };

        public GuildBattleCommonConst.GuildBattleGameState GameState = GuildBattleCommonConst.GuildBattleGameState.None;
        
        public long MatchingId { get; protected set; }
        public int MaxTurn { get; protected set; }
        public int RemainTurn { get; set; }
        public int TurnNumber { get; set; }
        public DateTime UtcStartAt { get; protected set; }
        public DateTime UtcEndAt { get; protected set; }

        private SemaphoreSlim sem = new SemaphoreSlim(1, 1);

        public Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, List<GuildBattleCommonChatData>> ReceivedChatData { get; private set; } =
            new Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, List<GuildBattleCommonChatData>>()
            {
                { GuildBattleCommonConst.GuildBattleTeamSide.Left, new List<GuildBattleCommonChatData>() },
                { GuildBattleCommonConst.GuildBattleTeamSide.Right, new List<GuildBattleCommonChatData>() }
            };
        
        protected GuildBattleCommonConst.GuildBattleTeamSide playerSide = GuildBattleCommonConst.GuildBattleTeamSide.All;
        public GuildBattleCommonConst.GuildBattleTeamSide PlayerSide => playerSide;
        
        protected int playerIndex = -1;
        public int PlayerIndex => playerIndex;
        
        public int PartyIdIndex = 0;
        public readonly Dictionary<long, int> PlayerPartyIdIndex = new Dictionary<long, int>();
        
        public void SetBattleTimeData(DateTime startAt, DateTime endAt)
        {
            UtcStartAt = startAt;
            UtcEndAt = endAt;
            MaxTurn = (int)((endAt - startAt).TotalMilliseconds / GuildBattleSetting.GuildBattlePerTurnDelayMilliseconds);
            RemainTurn = MaxTurn;
        }

        public void UpdateGameState(GuildBattleCommonConst.GuildBattleGameState gameState)
        {
            GameState = gameState;
        }
        #nullable enable
        public GuildBattleCommonPlayerData? GetBattlePlayerDataById(long playerId)
        {
            BattlePlayerDataById.TryGetValue(playerId, out var ret);
            return ret;
        }
        
        public GuildBattleCommonPlayerData? GetBattlePlayerData(int playerIndex)
        {
            BattlePlayerDataByIndex.TryGetValue(playerIndex, out var ret);
            return ret;
        }
        
        public GuildBattleUnitStateModel? GetUnitState(long unitId)
        {
            StandbyUnitDictionary.TryGetValue(unitId, out var ret);
            return ret;
        }
        #nullable disable
        
        public List<GuildBattleUnitStateModel> GetUnitStates(long playerIndex)
        {
            return StandbyUnits[playerIndex];
        }
        
        public bool IsClientOperatable()
        {
            return GameState is GuildBattleCommonConst.GuildBattleGameState.BeforeFight or GuildBattleCommonConst.GuildBattleGameState.InFight;
        }
        
        public bool IsInFight()
        {
            return GameState is GuildBattleCommonConst.GuildBattleGameState.InFight;
        }

        /// <summary>
        /// Referenced by MagicOnionServer
        /// </summary>
        public async Task<GuildBattleCommonChatData> AddChatData(GuildBattleCommonConst.GuildBattleTeamSide side, string body, long playerId, long stampId)
        {
            GuildBattleCommonChatData ret = null;
            try
            {
                await sem.WaitAsync();
                var target = PjfbGuildBattleDataMediator.Instance.ReceivedChatData[side];
                ret = new GuildBattleCommonChatData(target.Count, body, playerId, stampId, 0);
                target.Add(ret);
            }
            finally
            {
                sem.Release();
            }

            return ret;
        }
        
        /// <summary>
        /// Referenced by MagicOnionServer
        /// </summary>
        public async Task<GuildBattleCommonChatData> AddSystemChatData(GuildBattleCommonConst.GuildBattleTeamSide side, GuildBattleCommonChatData.SystemLogType logType, long playerId, long itemId)
        {
            GuildBattleCommonChatData ret = null;
            try
            {
                await sem.WaitAsync();
                var target = PjfbGuildBattleDataMediator.Instance.ReceivedChatData[side];
                ret = new GuildBattleCommonChatData(target.Count, (int)logType, playerId, itemId);
                target.Add(ret);
            }
            finally
            {
                sem.Release();
            }

            return ret;
        }
        
        public bool TryAddReceivedChatData(GuildBattleCommonChatData newChatData)
        {
            var sameChatData = ReceivedChatData[PlayerSide].FirstOrDefault(chatData => chatData.Index == newChatData.Index);
            if (sameChatData != null)
            {
                return false;
            }

            // 個別送信では対象プレイヤーIDにしか送っていないが, 接続時の全送信ではブロックしていないので受信側で弾く.
            if (newChatData.LogType == (int)GuildBattleCommonChatData.SystemLogType.PartyStartFighting)
            {
                var player =  GetBattlePlayerDataById(newChatData.SystemLogUserId2);
                if (player == null || playerIndex != player.PlayerIndex)
                {
                    return false;
                }
            }
            ReceivedChatData[PlayerSide].Add(newChatData);
            return true;
        }

        public virtual void AddBattleParty(GuildBattleCommonPartyModel party)
        {
            BattleParties.Add(party.PartyId, party);
        }
        
        public virtual void RemoveBattleParty(GuildBattleCommonPartyModel party)
        {
            BattleParties.Remove(party.PartyId);
        }
        
        public void UpdateBattleParties(List<GuildBattleCommonPartyModel> parties)
        {
            BattleParties.Clear();
            foreach (var party in parties)
            {
                BattleParties.Add(party.PartyId, party);
            }
        }

        /// <summary>
        /// Referenced by MagicOnionServer
        /// </summary>
        public async Task AddWinningPoint(GuildBattleCommonConst.GuildBattleTeamSide side, int addValue)
        {
            try
            {
                await sem.WaitAsync();
                WinningPoints[(int)side] += addValue;
            }
            finally
            {
                sem.Release();
            }
        }

        public void UpdateWinningPoints(int[] winningPoints)
        {
            WinningPoints[(int)GuildBattleCommonConst.GuildBattleTeamSide.Left] = winningPoints[(int)GuildBattleCommonConst.GuildBattleTeamSide.Left];
            WinningPoints[(int)GuildBattleCommonConst.GuildBattleTeamSide.Right] = winningPoints[(int)GuildBattleCommonConst.GuildBattleTeamSide.Right];
        }
    }
}