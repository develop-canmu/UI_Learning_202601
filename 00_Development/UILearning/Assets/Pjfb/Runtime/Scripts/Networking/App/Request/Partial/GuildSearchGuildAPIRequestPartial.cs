using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildSearchGuildAPIRequest : AppAPIRequestBase<GuildSearchGuildAPIPost, GuildSearchGuildAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildSearchGuildAPIResponse response ) {
        }
    }
}
