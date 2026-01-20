using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildCancelInvitationAPIRequest : AppAPIRequestBase<GuildCancelInvitationAPIPost, GuildCancelInvitationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildCancelInvitationAPIResponse response ) {
        }
    }
}
