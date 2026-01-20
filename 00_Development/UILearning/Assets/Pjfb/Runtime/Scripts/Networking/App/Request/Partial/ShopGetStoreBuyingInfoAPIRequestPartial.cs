using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ShopGetStoreBuyingInfoAPIRequest : AppAPIRequestBase<ShopGetStoreBuyingInfoAPIPost, ShopGetStoreBuyingInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ShopGetStoreBuyingInfoAPIResponse response ) {
        }
    }
}
