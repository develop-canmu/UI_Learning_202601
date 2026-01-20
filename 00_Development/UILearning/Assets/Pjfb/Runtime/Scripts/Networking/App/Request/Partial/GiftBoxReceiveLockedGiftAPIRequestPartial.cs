using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxReceiveLockedGiftAPIRequest : AppAPIRequestBase<GiftBoxReceiveLockedGiftAPIPost, GiftBoxReceiveLockedGiftAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxReceiveLockedGiftAPIResponse response ) {
        }
    }
}
