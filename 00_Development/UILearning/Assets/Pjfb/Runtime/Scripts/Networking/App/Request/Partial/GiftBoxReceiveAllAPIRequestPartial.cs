using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxReceiveAllAPIRequest : AppAPIRequestBase<GiftBoxReceiveAllAPIPost, GiftBoxReceiveAllAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxReceiveAllAPIResponse response ) {
        }
    }
}
