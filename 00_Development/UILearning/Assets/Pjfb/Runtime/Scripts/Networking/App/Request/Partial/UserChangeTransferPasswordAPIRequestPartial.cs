using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserChangeTransferPasswordAPIRequest : AppAPIRequestBase<UserChangeTransferPasswordAPIPost, UserChangeTransferPasswordAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserChangeTransferPasswordAPIResponse response ) {
        }
    }
}
