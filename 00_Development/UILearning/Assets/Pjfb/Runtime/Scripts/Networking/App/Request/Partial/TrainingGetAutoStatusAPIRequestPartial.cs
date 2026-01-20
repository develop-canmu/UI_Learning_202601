using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingGetAutoStatusAPIRequest : AppAPIRequestBase<TrainingGetAutoStatusAPIPost, TrainingGetAutoStatusAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingGetAutoStatusAPIResponse response ) {
        }
    }
}
