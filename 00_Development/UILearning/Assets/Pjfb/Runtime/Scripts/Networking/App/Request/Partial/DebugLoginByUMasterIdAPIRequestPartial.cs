using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugLoginByUMasterIdAPIRequest : AppAPIRequestBase<DebugLoginByUMasterIdAPIPost, DebugLoginByUMasterIdAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugLoginByUMasterIdAPIResponse response ) {
        }
    }
}
