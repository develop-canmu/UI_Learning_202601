using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaBoxDetailAPIRequest : AppAPIRequestBase<GachaBoxDetailAPIPost, GachaBoxDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaBoxDetailAPIResponse response ) {
        }
    }
}
