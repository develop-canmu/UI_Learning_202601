using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugDecodeDataAPIRequest : AppAPIRequestBase<DebugDecodeDataAPIPost, DebugDecodeDataAPIResponse>
    {
        
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugDecodeDataAPIResponse response ) {
        }
    }
}
