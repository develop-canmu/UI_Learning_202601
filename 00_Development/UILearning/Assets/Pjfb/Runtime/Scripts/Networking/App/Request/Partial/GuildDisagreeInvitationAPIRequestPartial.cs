using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildDisagreeInvitationAPIRequest : AppAPIRequestBase<GuildDisagreeInvitationAPIPost, GuildDisagreeInvitationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildDisagreeInvitationAPIResponse response ) {
        }
    }
}
