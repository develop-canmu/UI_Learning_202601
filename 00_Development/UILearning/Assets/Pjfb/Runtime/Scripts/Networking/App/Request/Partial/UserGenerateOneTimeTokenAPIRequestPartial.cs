using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserGenerateOneTimeTokenAPIRequest : AppAPIRequestBase<UserGenerateOneTimeTokenAPIPost, UserGenerateOneTimeTokenAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserGenerateOneTimeTokenAPIResponse response ) {
        }
    }
}
