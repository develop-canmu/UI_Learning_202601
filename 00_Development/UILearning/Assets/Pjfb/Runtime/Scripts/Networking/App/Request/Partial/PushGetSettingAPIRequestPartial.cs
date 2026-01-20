using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class PushGetSettingAPIRequest : AppAPIRequestBase<PushGetSettingAPIPost, PushGetSettingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( PushGetSettingAPIResponse response ) {
        }
    }
}
