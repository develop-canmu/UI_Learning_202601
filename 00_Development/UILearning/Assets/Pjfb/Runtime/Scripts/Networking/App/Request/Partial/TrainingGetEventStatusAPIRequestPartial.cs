using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingGetEventStatusAPIRequest : AppAPIRequestBase<TrainingGetEventStatusAPIPost, TrainingGetEventStatusAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingGetEventStatusAPIResponse response ) {
        }
    }
}
