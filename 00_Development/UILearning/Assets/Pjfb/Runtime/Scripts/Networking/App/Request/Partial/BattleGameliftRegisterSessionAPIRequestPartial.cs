using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftRegisterSessionAPIRequest : AppAPIRequestBase<BattleGameliftRegisterSessionAPIPost, BattleGameliftRegisterSessionAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftRegisterSessionAPIResponse response ) {
        }
    }
}
