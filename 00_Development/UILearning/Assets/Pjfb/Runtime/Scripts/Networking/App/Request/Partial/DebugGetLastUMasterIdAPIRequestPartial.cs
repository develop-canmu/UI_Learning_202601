using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetLastUMasterIdAPIRequest : AppAPIRequestBase<DebugGetLastUMasterIdAPIPost, DebugGetLastUMasterIdAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetLastUMasterIdAPIResponse response ) {
        }
    }
}
