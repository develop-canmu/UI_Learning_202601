using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserGetTransferSettingAPIRequest : AppAPIRequestBase<UserGetTransferSettingAPIPost, UserGetTransferSettingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserGetTransferSettingAPIResponse response ) {
        }
    }
}
