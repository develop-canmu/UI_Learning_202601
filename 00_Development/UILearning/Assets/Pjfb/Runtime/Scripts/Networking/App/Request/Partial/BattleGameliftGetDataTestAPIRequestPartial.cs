using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftGetDataTestAPIRequest : AppAPIRequestBase<BattleGameliftGetDataTestAPIPost, BattleGameliftGetDataTestAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftGetDataTestAPIResponse response ) {
        }
    }
}
