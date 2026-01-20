using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumReadFinishedListAPIRequest : AppAPIRequestBase<ColosseumReadFinishedListAPIPost, ColosseumReadFinishedListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumReadFinishedListAPIResponse response ) {
        }
    }
}
