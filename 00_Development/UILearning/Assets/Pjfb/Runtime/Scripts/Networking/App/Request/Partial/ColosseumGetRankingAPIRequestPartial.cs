using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetRankingAPIRequest : AppAPIRequestBase<ColosseumGetRankingAPIPost, ColosseumGetRankingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetRankingAPIResponse response ) {
        }
    }
}
