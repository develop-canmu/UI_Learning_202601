using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumCancelEntryAPIRequest : AppAPIRequestBase<ColosseumCancelEntryAPIPost, ColosseumCancelEntryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumCancelEntryAPIResponse response ) {
        }
    }
}
