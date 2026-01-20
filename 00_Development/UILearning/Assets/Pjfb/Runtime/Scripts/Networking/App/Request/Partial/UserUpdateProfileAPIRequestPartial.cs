using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserUpdateProfileAPIRequest : AppAPIRequestBase<UserUpdateProfileAPIPost, UserUpdateProfileAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserUpdateProfileAPIResponse response ) {
        }
    }
}
