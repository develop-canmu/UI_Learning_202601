using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetScorePlanedDetailAPIRequest : AppAPIRequestBase<ColosseumGetScorePlanedDetailAPIPost, ColosseumGetScorePlanedDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetScorePlanedDetailAPIResponse response ) {
        }
    }
}
