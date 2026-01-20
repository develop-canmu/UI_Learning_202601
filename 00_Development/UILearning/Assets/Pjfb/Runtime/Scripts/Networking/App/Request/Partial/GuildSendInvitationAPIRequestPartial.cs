using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildSendInvitationAPIRequest : AppAPIRequestBase<GuildSendInvitationAPIPost, GuildSendInvitationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildSendInvitationAPIResponse response ) {
        }
    }
}
