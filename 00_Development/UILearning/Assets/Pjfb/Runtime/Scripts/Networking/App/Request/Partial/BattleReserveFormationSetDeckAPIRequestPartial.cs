using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationSetDeckAPIRequest : AppAPIRequestBase<BattleReserveFormationSetDeckAPIPost, BattleReserveFormationSetDeckAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationSetDeckAPIResponse response ) {
        }
    }
}
