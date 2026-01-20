using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetMasterConfigAPIRequest : AppAPIRequestBase<DebugGetMasterConfigAPIPost, DebugGetMasterConfigAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetMasterConfigAPIResponse response ) {
        }
    }
}
