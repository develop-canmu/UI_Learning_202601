using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugGetTrainingBattleCountAndTurnListAPIRequest : AppAPIRequestBase<DebugGetTrainingBattleCountAndTurnListAPIPost, DebugGetTrainingBattleCountAndTurnListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugGetTrainingBattleCountAndTurnListAPIResponse response ) {
        }
    }
}
