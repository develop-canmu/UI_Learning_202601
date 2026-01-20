using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildKickAPIRequest : AppAPIRequestBase<GuildKickAPIPost, GuildKickAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildKickAPIResponse response ) {
        }
    }
}
