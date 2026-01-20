using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetTargetListAPIRequest : AppAPIRequestBase<ColosseumGetTargetListAPIPost, ColosseumGetTargetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetTargetListAPIResponse response ) {
        }
    }
}
