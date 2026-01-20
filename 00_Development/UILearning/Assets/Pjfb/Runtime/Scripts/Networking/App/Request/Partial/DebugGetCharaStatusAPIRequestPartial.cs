using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetCharaStatusAPIRequest : AppAPIRequestBase<DebugGetCharaStatusAPIPost, DebugGetCharaStatusAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetCharaStatusAPIResponse response ) {
        }
    }
}
