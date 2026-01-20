using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftBoxGetGiftHistoryListAPIRequest : AppAPIRequestBase<GiftBoxGetGiftHistoryListAPIPost, GiftBoxGetGiftHistoryListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftBoxGetGiftHistoryListAPIResponse response ) {
        }
    }
}
