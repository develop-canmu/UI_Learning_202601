using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserValidateNameAPIRequest : AppAPIRequestBase<UserValidateNameAPIPost, UserValidateNameAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserValidateNameAPIResponse response ) {
        }
    }
}
