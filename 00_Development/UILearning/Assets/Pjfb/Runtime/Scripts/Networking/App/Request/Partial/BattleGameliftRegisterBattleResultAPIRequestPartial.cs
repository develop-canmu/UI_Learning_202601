using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftRegisterBattleResultAPIRequest : AppAPIRequestBase<BattleGameliftRegisterBattleResultAPIPost, BattleGameliftRegisterBattleResultAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftRegisterBattleResultAPIResponse response ) {
        }
    }
}
