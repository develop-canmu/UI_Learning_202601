using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxGetGiftListAPIRequest : AppAPIRequestBase<GiftBoxGetGiftListAPIPost, GiftBoxGetGiftListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxGetGiftListAPIResponse response ) {
        }
    }
}
