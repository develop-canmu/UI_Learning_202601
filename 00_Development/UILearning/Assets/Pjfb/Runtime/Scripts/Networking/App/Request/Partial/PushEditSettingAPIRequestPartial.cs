using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PushEditSettingAPIRequest : AppAPIRequestBase<PushEditSettingAPIPost, PushEditSettingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PushEditSettingAPIResponse response ) {
        }
    }
}
