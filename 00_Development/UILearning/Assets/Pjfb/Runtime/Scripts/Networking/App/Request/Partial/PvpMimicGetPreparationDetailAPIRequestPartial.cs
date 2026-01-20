using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PvpMimicGetPreparationDetailAPIRequest : AppAPIRequestBase<PvpMimicGetPreparationDetailAPIPost, PvpMimicGetPreparationDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PvpMimicGetPreparationDetailAPIResponse response ) {
        }
    }
}
