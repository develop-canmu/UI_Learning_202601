using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MissionReceiveCategoryAPIRequest : AppAPIRequestBase<MissionReceiveCategoryAPIPost, MissionReceiveCategoryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MissionReceiveCategoryAPIResponse response ) {
        }
    }
}
