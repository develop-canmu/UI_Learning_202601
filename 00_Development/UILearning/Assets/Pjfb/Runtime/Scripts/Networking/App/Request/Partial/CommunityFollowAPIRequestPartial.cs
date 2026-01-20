using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityFollowAPIRequest : AppAPIRequestBase<CommunityFollowAPIPost, CommunityFollowAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityFollowAPIResponse response ) {
        }
    }
}
