using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopLotStoreAPIRequest : AppAPIRequestBase<ShopLotStoreAPIPost, ShopLotStoreAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopLotStoreAPIResponse response ) {
        }
    }
}
