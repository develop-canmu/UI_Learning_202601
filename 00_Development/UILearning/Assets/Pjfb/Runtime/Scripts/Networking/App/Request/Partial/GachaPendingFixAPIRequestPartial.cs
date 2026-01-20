using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaPendingFixAPIRequest : AppAPIRequestBase<GachaPendingFixAPIPost, GachaPendingFixAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaPendingFixAPIResponse response ) {
        }
    }
}
