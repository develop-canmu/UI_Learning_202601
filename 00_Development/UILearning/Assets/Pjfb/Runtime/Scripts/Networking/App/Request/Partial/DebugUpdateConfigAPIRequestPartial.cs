using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugUpdateConfigAPIRequest : AppAPIRequestBase<DebugUpdateConfigAPIPost, DebugUpdateConfigAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugUpdateConfigAPIResponse response ) {
        }
    }
}
