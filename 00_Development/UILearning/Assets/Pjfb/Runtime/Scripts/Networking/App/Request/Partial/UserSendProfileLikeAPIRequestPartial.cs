using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserSendProfileLikeAPIRequest : AppAPIRequestBase<UserSendProfileLikeAPIPost, UserSendProfileLikeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserSendProfileLikeAPIResponse response ) {
        }
    }
}
