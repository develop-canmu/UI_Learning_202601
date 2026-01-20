using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaReviewShowAPIRequest : AppAPIRequestBase<GachaReviewShowAPIPost, GachaReviewShowAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaReviewShowAPIResponse response ) {
        }
    }
}
