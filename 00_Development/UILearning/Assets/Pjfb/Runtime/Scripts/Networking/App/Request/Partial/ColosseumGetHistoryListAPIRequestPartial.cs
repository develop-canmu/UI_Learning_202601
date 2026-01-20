using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetHistoryListAPIRequest : AppAPIRequestBase<ColosseumGetHistoryListAPIPost, ColosseumGetHistoryListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetHistoryListAPIResponse response ) {
        }
    }
}
