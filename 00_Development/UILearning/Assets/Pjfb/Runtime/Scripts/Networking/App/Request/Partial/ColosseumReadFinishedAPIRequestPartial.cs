using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumReadFinishedAPIRequest : AppAPIRequestBase<ColosseumReadFinishedAPIPost, ColosseumReadFinishedAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumReadFinishedAPIResponse response ) {
        }
    }
}
