using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class RankingGetGuildCommonRankListAPIRequest : AppAPIRequestBase<RankingGetGuildCommonRankListAPIPost, RankingGetGuildCommonRankListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( RankingGetGuildCommonRankListAPIResponse response ) {
        }
    }
}
