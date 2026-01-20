using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserInitializeAPIRequest : AppAPIRequestBase<UserInitializeAPIPost, UserInitializeAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserInitializeAPIResponse response ) {
        }
    }
}
