using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugTestAPIRequest : AppAPIRequestBase<DebugTestAPIPost, DebugTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugTestAPIResponse response ) {
        }
    }
}
