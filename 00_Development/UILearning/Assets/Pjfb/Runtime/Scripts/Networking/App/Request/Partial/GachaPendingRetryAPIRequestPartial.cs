using Pjfb.Gacha;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaPendingRetryAPIRequest : AppAPIRequestBase<GachaPendingRetryAPIPost, GachaPendingRetryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaPendingRetryAPIResponse response ) {
        }
    }
}
