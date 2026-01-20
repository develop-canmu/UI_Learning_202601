using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetMatchInfoAPIRequest : AppAPIRequestBase<BattleReserveFormationGetMatchInfoAPIPost, BattleReserveFormationGetMatchInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetMatchInfoAPIResponse response ) {
        }
    }
}
