using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopSelectBillingRewardAPIRequest : AppAPIRequestBase<ShopSelectBillingRewardAPIPost, ShopSelectBillingRewardAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopSelectBillingRewardAPIResponse response ) {
        }
    }
}
