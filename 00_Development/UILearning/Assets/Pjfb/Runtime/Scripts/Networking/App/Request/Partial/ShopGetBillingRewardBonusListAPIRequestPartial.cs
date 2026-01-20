using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopGetBillingRewardBonusListAPIRequest : AppAPIRequestBase<ShopGetBillingRewardBonusListAPIPost, ShopGetBillingRewardBonusListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopGetBillingRewardBonusListAPIResponse response ) {
        }
    }
}
