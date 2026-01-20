using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class RankingGetGuildAPIRequest : AppAPIRequestBase<RankingGetGuildAPIPost, RankingGetGuildAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( RankingGetGuildAPIResponse response ) {
        }
    }
}
