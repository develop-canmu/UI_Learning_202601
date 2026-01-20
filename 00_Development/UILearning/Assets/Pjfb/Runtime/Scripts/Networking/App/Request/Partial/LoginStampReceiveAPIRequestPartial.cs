using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class LoginStampReceiveAPIRequest : AppAPIRequestBase<LoginStampReceiveAPIPost, LoginStampReceiveAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( LoginStampReceiveAPIResponse response ) {
        }
    }
}
