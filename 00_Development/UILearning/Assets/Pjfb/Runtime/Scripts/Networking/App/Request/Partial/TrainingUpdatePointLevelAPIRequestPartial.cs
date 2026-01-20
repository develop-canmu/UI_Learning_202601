using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingUpdatePointLevelAPIRequest : AppAPIRequestBase<TrainingUpdatePointLevelAPIPost, TrainingUpdatePointLevelAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingUpdatePointLevelAPIResponse response ) {
        }
    }
}
