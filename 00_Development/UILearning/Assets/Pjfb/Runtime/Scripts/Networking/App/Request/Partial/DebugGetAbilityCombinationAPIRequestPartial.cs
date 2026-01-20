using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetAbilityCombinationAPIRequest : AppAPIRequestBase<DebugGetAbilityCombinationAPIPost, DebugGetAbilityCombinationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetAbilityCombinationAPIResponse response ) {
        }
    }
}
