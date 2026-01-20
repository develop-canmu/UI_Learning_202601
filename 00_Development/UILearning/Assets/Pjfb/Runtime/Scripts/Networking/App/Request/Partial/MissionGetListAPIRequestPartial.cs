using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MissionGetListAPIRequest : AppAPIRequestBase<MissionGetListAPIPost, MissionGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MissionGetListAPIResponse response ) {
        }
    }
}
