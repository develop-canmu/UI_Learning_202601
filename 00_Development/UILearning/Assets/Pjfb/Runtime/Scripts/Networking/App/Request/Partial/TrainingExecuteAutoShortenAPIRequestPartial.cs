using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingExecuteAutoShortenAPIRequest : AppAPIRequestBase<TrainingExecuteAutoShortenAPIPost, TrainingExecuteAutoShortenAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingExecuteAutoShortenAPIResponse response ) {
        }
    }
}
