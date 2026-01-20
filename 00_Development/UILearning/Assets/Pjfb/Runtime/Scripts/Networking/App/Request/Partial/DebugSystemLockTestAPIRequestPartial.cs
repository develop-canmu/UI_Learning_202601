using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugSystemLockTestAPIRequest : AppAPIRequestBase<DebugSystemLockTestAPIPost, DebugSystemLockTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugSystemLockTestAPIResponse response ) {
        }
    }
}
