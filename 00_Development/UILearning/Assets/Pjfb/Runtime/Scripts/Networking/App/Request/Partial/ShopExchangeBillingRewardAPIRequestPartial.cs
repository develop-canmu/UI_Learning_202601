using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopExchangeBillingRewardAPIRequest : AppAPIRequestBase<ShopExchangeBillingRewardAPIPost, ShopExchangeBillingRewardAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopExchangeBillingRewardAPIResponse response ) {
        }
    }
}
