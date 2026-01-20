using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntStartAPIRequest : AppAPIRequestBase<HuntStartAPIPost, HuntStartAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntStartAPIResponse response ) {
        }
    }
}
