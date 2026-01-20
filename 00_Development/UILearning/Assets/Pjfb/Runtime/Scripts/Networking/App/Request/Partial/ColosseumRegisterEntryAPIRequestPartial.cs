using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumRegisterEntryAPIRequest : AppAPIRequestBase<ColosseumRegisterEntryAPIPost, ColosseumRegisterEntryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumRegisterEntryAPIResponse response ) {
        }
    }
}
