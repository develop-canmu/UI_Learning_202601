using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugErrorAPIRequest : AppAPIRequestBase<DebugErrorAPIPost, DebugErrorAPIResponse>
    {

        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugErrorAPIResponse response ) {
        }
    }
}
