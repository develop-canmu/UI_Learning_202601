using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MissionGetSuccessCountAPIRequest : AppAPIRequestBase<MissionGetSuccessCountAPIPost, MissionGetSuccessCountAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MissionGetSuccessCountAPIResponse response ) {
        }
    }
}
