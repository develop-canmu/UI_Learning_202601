using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetHistoryDeckAPIRequest : AppAPIRequestBase<ColosseumGetHistoryDeckAPIPost, ColosseumGetHistoryDeckAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetHistoryDeckAPIResponse response ) {
        }
    }
}
