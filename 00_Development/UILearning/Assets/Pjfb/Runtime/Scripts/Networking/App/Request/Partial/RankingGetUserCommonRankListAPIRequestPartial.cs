using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class RankingGetUserCommonRankListAPIRequest : AppAPIRequestBase<RankingGetUserCommonRankListAPIPost, RankingGetUserCommonRankListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( RankingGetUserCommonRankListAPIResponse response ) {
        }
    }
}
