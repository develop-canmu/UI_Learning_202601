using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetDeckAPIRequest : AppAPIRequestBase<ColosseumGetDeckAPIPost, ColosseumGetDeckAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetDeckAPIResponse response ) {
        }
    }
}
