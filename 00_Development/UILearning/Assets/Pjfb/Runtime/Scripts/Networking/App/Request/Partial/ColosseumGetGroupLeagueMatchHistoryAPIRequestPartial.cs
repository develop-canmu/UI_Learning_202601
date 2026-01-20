using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetGroupLeagueMatchHistoryAPIRequest : AppAPIRequestBase<ColosseumGetGroupLeagueMatchHistoryAPIPost, ColosseumGetGroupLeagueMatchHistoryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetGroupLeagueMatchHistoryAPIResponse response ) {
        }
    }
}
