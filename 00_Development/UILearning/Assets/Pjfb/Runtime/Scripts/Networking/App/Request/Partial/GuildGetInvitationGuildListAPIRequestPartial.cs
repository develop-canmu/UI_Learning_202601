using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildGetInvitationGuildListAPIRequest : AppAPIRequestBase<GuildGetInvitationGuildListAPIPost, GuildGetInvitationGuildListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildGetInvitationGuildListAPIResponse response ) {
            UserData.UserDataManager.Instance.user.Update(response);
        }
    }
}
