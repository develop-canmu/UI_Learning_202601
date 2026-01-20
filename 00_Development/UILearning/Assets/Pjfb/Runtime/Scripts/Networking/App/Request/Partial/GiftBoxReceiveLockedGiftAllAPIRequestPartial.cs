using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxReceiveLockedGiftAllAPIRequest : AppAPIRequestBase<GiftBoxReceiveLockedGiftAllAPIPost, GiftBoxReceiveLockedGiftAllAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxReceiveLockedGiftAllAPIResponse response ) {
        }
    }
}
