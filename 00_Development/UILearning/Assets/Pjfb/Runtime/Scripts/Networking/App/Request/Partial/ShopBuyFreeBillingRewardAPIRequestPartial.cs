using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopBuyFreeBillingRewardAPIRequest : AppAPIRequestBase<ShopBuyFreeBillingRewardAPIPost, ShopBuyFreeBillingRewardAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopBuyFreeBillingRewardAPIResponse response ) {
        }
    }
}
