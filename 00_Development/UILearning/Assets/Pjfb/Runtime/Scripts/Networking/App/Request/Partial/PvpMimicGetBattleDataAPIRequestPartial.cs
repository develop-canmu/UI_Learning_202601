using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PvpMimicGetBattleDataAPIRequest : AppAPIRequestBase<PvpMimicGetBattleDataAPIPost, PvpMimicGetBattleDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PvpMimicGetBattleDataAPIResponse response ) {
        }
    }
}
