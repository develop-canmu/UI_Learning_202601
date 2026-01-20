using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildGetInvitationUserListAPIRequest : AppAPIRequestBase<GuildGetInvitationUserListAPIPost, GuildGetInvitationUserListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildGetInvitationUserListAPIResponse response ) {
        }
    }
}
