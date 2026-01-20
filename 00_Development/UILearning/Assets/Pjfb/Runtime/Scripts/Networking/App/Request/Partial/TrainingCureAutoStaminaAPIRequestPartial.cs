using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingCureAutoStaminaAPIRequest : AppAPIRequestBase<TrainingCureAutoStaminaAPIPost, TrainingCureAutoStaminaAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingCureAutoStaminaAPIResponse response ) {
        }
    }
}
