using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetGroupBattleRankingAPIRequest : AppAPIRequestBase<ColosseumGetGroupBattleRankingAPIPost, ColosseumGetGroupBattleRankingAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetGroupBattleRankingAPIResponse response ) {
        }
    }
}
