using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfb;
using Pjfb.Networking.App.Request;

namespace MagicOnion
{

    /// <summary>
    /// Server -> Client
    /// メソッドの戻り値はvoidのみ. サーバからパラメータに値を渡していく. ただし許容されるパラメータの最大数は15
    /// </summary>
    public interface IPjfbGameHubReceiver : ICommonGameHubReceiver
    {
        void OnReceiveBattleData(BattleV2ClientData battleData, DateTime utcStartAt);
        void OnReceiveBattleTurnResult(GuildBattleFieldSituationModel fieldSituation);
        void OnReceiveBattlePlayerData(GuildBattlePlayerData battlePlayerData, bool isTurnResult);
        void OnReceiveBattleResult(GuildBattleResultData resultData);
        void OnReceivePlayerPartyData(List<GuildBattlePartyModel> parties);
    }

    /// <summary>
    /// Client -> Server
    /// </summary>
    public interface IPjfbGameHub : IStreamingHub<IPjfbGameHub, IPjfbGameHubReceiver>, ICommonGameHub
    {
        ValueTask<GuildBattlePlayerData> RequestSendParty(SendPartyCommand command);
        ValueTask<GuildBattlePlayerData> RequestUseItem(long itemId);
        ValueTask<GuildBattlePlayerData> RequestUseYellAbility(long characterId, long abilityId);

    }
}