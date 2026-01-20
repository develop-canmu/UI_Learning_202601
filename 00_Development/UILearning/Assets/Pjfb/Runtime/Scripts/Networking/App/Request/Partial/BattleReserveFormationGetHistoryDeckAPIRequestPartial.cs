using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetHistoryDeckAPIRequest : AppAPIRequestBase<BattleReserveFormationGetHistoryDeckAPIPost, BattleReserveFormationGetHistoryDeckAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetHistoryDeckAPIResponse response ) {
        }
    }
}
