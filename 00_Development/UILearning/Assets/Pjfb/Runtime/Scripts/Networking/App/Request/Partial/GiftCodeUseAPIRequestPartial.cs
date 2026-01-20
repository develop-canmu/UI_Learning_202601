using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GiftCodeUseAPIRequest : AppAPIRequestBase<GiftCodeUseAPIPost, GiftCodeUseAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GiftCodeUseAPIResponse response ) {
        }
    }
}
