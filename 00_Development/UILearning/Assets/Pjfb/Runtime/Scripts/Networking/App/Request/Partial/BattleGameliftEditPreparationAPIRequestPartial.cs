using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftEditPreparationAPIRequest : AppAPIRequestBase<BattleGameliftEditPreparationAPIPost, BattleGameliftEditPreparationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftEditPreparationAPIResponse response ) {
        }
    }
}
