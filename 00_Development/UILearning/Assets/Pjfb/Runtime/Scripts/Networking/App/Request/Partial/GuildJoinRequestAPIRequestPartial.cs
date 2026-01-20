using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildJoinRequestAPIRequest : AppAPIRequestBase<GuildJoinRequestAPIPost, GuildJoinRequestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildJoinRequestAPIResponse response ) {
        }
    }
}
