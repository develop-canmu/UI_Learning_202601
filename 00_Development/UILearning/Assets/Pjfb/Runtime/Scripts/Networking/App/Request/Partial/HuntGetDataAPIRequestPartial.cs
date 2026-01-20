using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntGetDataAPIRequest : AppAPIRequestBase<HuntGetDataAPIPost, HuntGetDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntGetDataAPIResponse response ) {
        }
    }
}
