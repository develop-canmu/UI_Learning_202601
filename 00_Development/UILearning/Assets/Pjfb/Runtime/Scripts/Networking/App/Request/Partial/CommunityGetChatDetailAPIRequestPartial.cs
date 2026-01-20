using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetChatDetailAPIRequest : AppAPIRequestBase<CommunityGetChatDetailAPIPost, CommunityGetChatDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetChatDetailAPIResponse response ) {
        }
    }
}
