using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopGetLotteryStoreInfoAPIRequest : AppAPIRequestBase<ShopGetLotteryStoreInfoAPIPost, ShopGetLotteryStoreInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopGetLotteryStoreInfoAPIResponse response ) {
        }
    }
}
