using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetCacheNodeListAPIRequest : AppAPIRequestBase<DebugGetCacheNodeListAPIPost, DebugGetCacheNodeListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetCacheNodeListAPIResponse response ) {
        }
    }
}
