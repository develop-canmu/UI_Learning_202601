using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingFinishAutoAPIRequest : AppAPIRequestBase<TrainingFinishAutoAPIPost, TrainingFinishAutoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingFinishAutoAPIResponse response ) {
        }
    }
}
