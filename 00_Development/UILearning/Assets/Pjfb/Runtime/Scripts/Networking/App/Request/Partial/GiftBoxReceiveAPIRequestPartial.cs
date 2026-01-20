using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxReceiveAPIRequest : AppAPIRequestBase<GiftBoxReceiveAPIPost, GiftBoxReceiveAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxReceiveAPIResponse response ) {
        }
    }
}
