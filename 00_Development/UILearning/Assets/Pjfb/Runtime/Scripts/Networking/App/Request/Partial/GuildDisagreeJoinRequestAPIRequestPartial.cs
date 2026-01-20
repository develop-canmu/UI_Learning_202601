using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildDisagreeJoinRequestAPIRequest : AppAPIRequestBase<GuildDisagreeJoinRequestAPIPost, GuildDisagreeJoinRequestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildDisagreeJoinRequestAPIResponse response ) {
        }
    }
}
