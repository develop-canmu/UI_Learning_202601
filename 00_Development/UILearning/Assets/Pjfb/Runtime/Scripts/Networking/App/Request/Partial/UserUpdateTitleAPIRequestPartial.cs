using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserUpdateTitleAPIRequest : AppAPIRequestBase<UserUpdateTitleAPIPost, UserUpdateTitleAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserUpdateTitleAPIResponse response ) {
        }
    }
}
