using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxGetLockedGiftListAPIRequest : AppAPIRequestBase<GiftBoxGetLockedGiftListAPIPost, GiftBoxGetLockedGiftListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxGetLockedGiftListAPIResponse response ) {
        }
    }
}
