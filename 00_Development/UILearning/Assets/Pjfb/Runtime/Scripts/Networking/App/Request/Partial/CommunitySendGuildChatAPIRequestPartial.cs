using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunitySendGuildChatAPIRequest : AppAPIRequestBase<CommunitySendGuildChatAPIPost, CommunitySendGuildChatAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunitySendGuildChatAPIResponse response ) {
        }
    }
}
