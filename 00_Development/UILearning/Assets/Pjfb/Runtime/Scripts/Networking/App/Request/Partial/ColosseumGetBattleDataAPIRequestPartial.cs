using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetBattleDataAPIRequest : AppAPIRequestBase<ColosseumGetBattleDataAPIPost, ColosseumGetBattleDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetBattleDataAPIResponse response ) {
        }
    }
}
