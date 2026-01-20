using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleReserveFormationGetMatchInfoByEventInfoAPIRequest : AppAPIRequestBase<BattleReserveFormationGetMatchInfoByEventInfoAPIPost, BattleReserveFormationGetMatchInfoByEventInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleReserveFormationGetMatchInfoByEventInfoAPIResponse response ) {
        }
    }
}
