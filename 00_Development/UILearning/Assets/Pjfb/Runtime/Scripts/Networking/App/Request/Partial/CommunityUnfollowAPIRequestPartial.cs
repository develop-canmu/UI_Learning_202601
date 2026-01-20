using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityUnfollowAPIRequest : AppAPIRequestBase<CommunityUnfollowAPIPost, CommunityUnfollowAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityUnfollowAPIResponse response ) {
        }
    }
}
