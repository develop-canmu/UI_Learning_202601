using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugWithOneTimeTokenTestAPIRequest : AppAPIRequestBase<DebugWithOneTimeTokenTestAPIPost, DebugWithOneTimeTokenTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugWithOneTimeTokenTestAPIResponse response ) {
        }
    }
}
