using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingAbortAutoAPIRequest : AppAPIRequestBase<TrainingAbortAutoAPIPost, TrainingAbortAutoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingAbortAutoAPIResponse response ) {
        }
    }
}
