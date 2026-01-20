using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicOnion
{
    /// <summary>
    /// Server -> Client
    /// メソッドの戻り値はvoidのみ. サーバからパラメータに値を渡していく. ただし許容されるパラメータの最大数は15
    /// </summary>
    public interface ICommonGameHubReceiver
    {
        void OnJoin(GuildBattleCommonHubPlayerData player);
        void OnLeave(GuildBattleCommonHubPlayerData player);
        void OnReadyServer();
        
        void OnReceiveChat(GuildBattleCommonChatData commonChatData);
        void OnReceiveAllChat(List<GuildBattleCommonChatData> allChatData);
        void OnReceiveActivePlayerIds(List<long> playerIds);
        void OnReceiveCurrentGameState(GuildBattleCommonConst.GuildBattleGameState gameState);
    }

    /// <summary>
    /// Client -> Server
    /// </summary>
    public interface ICommonGameHub
    {
        ValueTask<GuildBattleCommonConst.JoinResult> JoinAsync(string roomName, long userId, long matchingId);
        ValueTask<GuildBattleCommonConst.JoinResult> DebugJoinAsync(string roomName, long userId, long leftGuildId, long rightGuildId, long deckType);

        ValueTask OnReady();
        ValueTask LeaveAsync();
        ValueTask<bool> RequestDissolutionParty(DissolutionPartyCommand command);
        ValueTask<bool> SendChat(string body, long stampId);
    }
}