using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class BattleGameliftSaveCloudLogAPIRequest : AppAPIRequestBase<BattleGameliftSaveCloudLogAPIPost, BattleGameliftSaveCloudLogAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( BattleGameliftSaveCloudLogAPIResponse response ) {
        }
    }
}
