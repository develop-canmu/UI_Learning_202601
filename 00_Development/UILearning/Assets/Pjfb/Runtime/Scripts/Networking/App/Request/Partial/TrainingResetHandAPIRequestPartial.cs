using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingResetHandAPIRequest : AppAPIRequestBase<TrainingResetHandAPIPost, TrainingResetHandAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingResetHandAPIResponse response ) {
        }
    }
}
