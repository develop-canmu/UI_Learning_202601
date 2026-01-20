using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunitySearchUserAPIRequest : AppAPIRequestBase<CommunitySearchUserAPIPost, CommunitySearchUserAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunitySearchUserAPIResponse response ) {
        }
    }
}
