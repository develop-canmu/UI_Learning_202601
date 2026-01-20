using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopUpdateParentalConsentAPIRequest : AppAPIRequestBase<ShopUpdateParentalConsentAPIPost, ShopUpdateParentalConsentAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopUpdateParentalConsentAPIResponse response ) {
        }
    }
}
