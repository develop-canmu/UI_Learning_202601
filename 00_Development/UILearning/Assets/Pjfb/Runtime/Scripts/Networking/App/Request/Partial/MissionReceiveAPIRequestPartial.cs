using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MissionReceiveAPIRequest : AppAPIRequestBase<MissionReceiveAPIPost, MissionReceiveAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MissionReceiveAPIResponse response ) {
        }
    }
}
