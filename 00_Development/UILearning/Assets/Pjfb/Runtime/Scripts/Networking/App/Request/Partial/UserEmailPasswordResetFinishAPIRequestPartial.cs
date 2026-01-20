using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserEmailPasswordResetFinishAPIRequest : AppAPIRequestBase<UserEmailPasswordResetFinishAPIPost, UserEmailPasswordResetFinishAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserEmailPasswordResetFinishAPIResponse response ) {
        }
    }
}
