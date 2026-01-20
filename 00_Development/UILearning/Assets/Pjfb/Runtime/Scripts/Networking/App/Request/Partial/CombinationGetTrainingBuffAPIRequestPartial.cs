using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CombinationGetTrainingBuffAPIRequest : AppAPIRequestBase<CombinationGetTrainingBuffAPIPost, CombinationGetTrainingBuffAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CombinationGetTrainingBuffAPIResponse response ) {
        }
    }
}
