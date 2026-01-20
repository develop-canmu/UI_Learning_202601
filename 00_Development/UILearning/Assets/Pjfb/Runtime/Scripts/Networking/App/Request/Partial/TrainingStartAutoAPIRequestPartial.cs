using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingStartAutoAPIRequest : AppAPIRequestBase<TrainingStartAutoAPIPost, TrainingStartAutoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingStartAutoAPIResponse response ) {
        }
    }
}
