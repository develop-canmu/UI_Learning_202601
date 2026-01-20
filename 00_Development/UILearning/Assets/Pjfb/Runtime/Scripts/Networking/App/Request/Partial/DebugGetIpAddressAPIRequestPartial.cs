using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetIpAddressAPIRequest : AppAPIRequestBase<DebugGetIpAddressAPIPost, DebugGetIpAddressAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetIpAddressAPIResponse response ) {
        }
    }
}
