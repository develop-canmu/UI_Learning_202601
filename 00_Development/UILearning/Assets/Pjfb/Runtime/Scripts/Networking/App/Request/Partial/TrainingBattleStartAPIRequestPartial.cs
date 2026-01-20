using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingBattleStartAPIRequest : AppAPIRequestBase<TrainingBattleStartAPIPost, TrainingBattleStartAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingBattleStartAPIResponse response ) {
        }
    }
}
