using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopUpdatePaymentLimitAPIRequest : AppAPIRequestBase<ShopUpdatePaymentLimitAPIPost, ShopUpdatePaymentLimitAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopUpdatePaymentLimitAPIResponse response ) {
        }
    }
}
