using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityBlockAPIRequest : AppAPIRequestBase<CommunityBlockAPIPost, CommunityBlockAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityBlockAPIResponse response ) {
        }
    }
}
