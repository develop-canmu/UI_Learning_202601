using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildJoinAPIRequest : AppAPIRequestBase<GuildJoinAPIPost, GuildJoinAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildJoinAPIResponse response ) {
        }
    }
}
