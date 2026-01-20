using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunitySendChatAPIRequest : AppAPIRequestBase<CommunitySendChatAPIPost, CommunitySendChatAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunitySendChatAPIResponse response ) {
        }
    }
}
