using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityDeleteChatAPIRequest : AppAPIRequestBase<CommunityDeleteChatAPIPost, CommunityDeleteChatAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityDeleteChatAPIResponse response ) {
        }
    }
}
