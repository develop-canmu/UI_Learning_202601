using Pjfb.Networking.API;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildCreateAPIRequest : AppAPIRequestBase<GuildCreateAPIPost, GuildCreateAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildCreateAPIResponse response ) {
            UserDataManager.Instance.user.UpdateGuildData(response.guild);
        }
    }
}
