using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildDissoluteAPIRequest : AppAPIRequestBase<GuildDissoluteAPIPost, GuildDissoluteAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildDissoluteAPIResponse response ) {
            UserData.UserDataManager.Instance.user.ClearGuildId();
        }
    }
}
