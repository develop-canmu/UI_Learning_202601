using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetHistoryListAPIRequest : AppAPIRequestBase<BattleReserveFormationGetHistoryListAPIPost, BattleReserveFormationGetHistoryListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetHistoryListAPIResponse response ) {
        }
    }
}
