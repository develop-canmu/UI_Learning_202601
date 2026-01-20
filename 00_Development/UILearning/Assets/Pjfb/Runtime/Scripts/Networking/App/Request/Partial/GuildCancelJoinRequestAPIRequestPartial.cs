using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildCancelJoinRequestAPIRequest : AppAPIRequestBase<GuildCancelJoinRequestAPIPost, GuildCancelJoinRequestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildCancelJoinRequestAPIResponse response ) {
        }
    }
}
