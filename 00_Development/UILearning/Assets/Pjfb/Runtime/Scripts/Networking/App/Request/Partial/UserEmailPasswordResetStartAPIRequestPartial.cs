using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserEmailPasswordResetStartAPIRequest : AppAPIRequestBase<UserEmailPasswordResetStartAPIPost, UserEmailPasswordResetStartAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserEmailPasswordResetStartAPIResponse response ) {
        }
    }
}
