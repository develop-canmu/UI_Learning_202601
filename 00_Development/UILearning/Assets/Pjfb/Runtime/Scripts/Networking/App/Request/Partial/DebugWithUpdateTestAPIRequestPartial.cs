using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugWithUpdateTestAPIRequest : AppAPIRequestBase<DebugWithUpdateTestAPIPost, DebugWithUpdateTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugWithUpdateTestAPIResponse response ) {
        }
    }
}
