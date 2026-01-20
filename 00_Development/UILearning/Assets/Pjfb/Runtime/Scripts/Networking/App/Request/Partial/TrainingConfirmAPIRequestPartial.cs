using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingConfirmAPIRequest : AppAPIRequestBase<TrainingConfirmAPIPost, TrainingConfirmAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingConfirmAPIResponse response ) {
        }
    }
}
