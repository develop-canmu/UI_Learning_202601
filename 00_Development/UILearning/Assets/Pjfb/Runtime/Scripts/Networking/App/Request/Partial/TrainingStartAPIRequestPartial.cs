using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingStartAPIRequest : AppAPIRequestBase<TrainingStartAPIPost, TrainingStartAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingStartAPIResponse response ) {
        }
    }
}
