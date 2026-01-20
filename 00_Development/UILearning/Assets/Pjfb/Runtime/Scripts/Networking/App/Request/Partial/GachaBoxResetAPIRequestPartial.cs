using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaBoxResetAPIRequest : AppAPIRequestBase<GachaBoxResetAPIPost, GachaBoxResetAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaBoxResetAPIResponse response ) {
        }
    }
}
