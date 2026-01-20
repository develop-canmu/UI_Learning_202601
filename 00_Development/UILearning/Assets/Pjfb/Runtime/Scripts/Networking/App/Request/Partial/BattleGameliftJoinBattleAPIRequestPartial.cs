using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftJoinBattleAPIRequest : AppAPIRequestBase<BattleGameliftJoinBattleAPIPost, BattleGameliftJoinBattleAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftJoinBattleAPIResponse response ) {
        }
    }
}
