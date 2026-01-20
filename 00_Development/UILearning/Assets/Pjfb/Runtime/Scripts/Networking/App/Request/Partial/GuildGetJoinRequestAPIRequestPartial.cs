using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildGetJoinRequestAPIRequest : AppAPIRequestBase<GuildGetJoinRequestAPIPost, GuildGetJoinRequestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildGetJoinRequestAPIResponse response ) {
        }
    }
}
