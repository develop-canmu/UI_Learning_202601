using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntCancelAPIRequest : AppAPIRequestBase<HuntCancelAPIPost, HuntCancelAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntCancelAPIResponse response ) {
        }
    }
}
