using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingProgressAPIRequest : AppAPIRequestBase<TrainingProgressAPIPost, TrainingProgressAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingProgressAPIResponse response ) {
        }
    }
}
