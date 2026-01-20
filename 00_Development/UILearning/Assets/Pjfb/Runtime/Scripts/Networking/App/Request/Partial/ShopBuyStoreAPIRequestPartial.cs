using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopBuyStoreAPIRequest : AppAPIRequestBase<ShopBuyStoreAPIPost, ShopBuyStoreAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopBuyStoreAPIResponse response ) {
        }
    }
}
