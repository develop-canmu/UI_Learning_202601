using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugPageDeployTestAPIRequest : AppAPIRequestBase<DebugPageDeployTestAPIPost, DebugPageDeployTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugPageDeployTestAPIResponse response ) {
        }
    }
}
