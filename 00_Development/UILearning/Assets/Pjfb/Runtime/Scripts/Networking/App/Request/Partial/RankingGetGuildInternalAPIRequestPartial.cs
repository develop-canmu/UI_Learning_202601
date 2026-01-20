using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class RankingGetGuildInternalAPIRequest : AppAPIRequestBase<RankingGetGuildInternalAPIPost, RankingGetGuildInternalAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( RankingGetGuildInternalAPIResponse response ) {
        }
    }
}
