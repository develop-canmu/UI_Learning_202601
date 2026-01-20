using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftGetMatchingInfoAPIRequest : AppAPIRequestBase<BattleGameliftGetMatchingInfoAPIPost, BattleGameliftGetMatchingInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftGetMatchingInfoAPIResponse response ) {
        }
    }
}
