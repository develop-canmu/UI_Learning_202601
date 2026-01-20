using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GuildUpdateRoleAPIRequest : AppAPIRequestBase<GuildUpdateRoleAPIPost, GuildUpdateRoleAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GuildUpdateRoleAPIResponse response ) {
        }
    }
}
