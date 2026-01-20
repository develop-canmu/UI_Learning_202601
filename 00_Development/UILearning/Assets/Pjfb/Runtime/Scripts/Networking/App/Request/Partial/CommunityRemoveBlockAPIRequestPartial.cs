using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityRemoveBlockAPIRequest : AppAPIRequestBase<CommunityRemoveBlockAPIPost, CommunityRemoveBlockAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityRemoveBlockAPIResponse response ) {
        }
    }
}
