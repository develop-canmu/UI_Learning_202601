using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildSearchInvitationUserAPIRequest : AppAPIRequestBase<GuildSearchInvitationUserAPIPost, GuildSearchInvitationUserAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildSearchInvitationUserAPIResponse response ) {
        }
    }
}
