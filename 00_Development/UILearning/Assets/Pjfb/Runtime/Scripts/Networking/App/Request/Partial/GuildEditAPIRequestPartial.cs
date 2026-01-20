using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildEditAPIRequest : AppAPIRequestBase<GuildEditAPIPost, GuildEditAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildEditAPIResponse response ) {
        }
    }
}
