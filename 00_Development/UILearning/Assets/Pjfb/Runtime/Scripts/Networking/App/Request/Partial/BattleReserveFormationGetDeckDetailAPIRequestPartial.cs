using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetDeckDetailAPIRequest : AppAPIRequestBase<BattleReserveFormationGetDeckDetailAPIPost, BattleReserveFormationGetDeckDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetDeckDetailAPIResponse response ) {
        }
    }
}
