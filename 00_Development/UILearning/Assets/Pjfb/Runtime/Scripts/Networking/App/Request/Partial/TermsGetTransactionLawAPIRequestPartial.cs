using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsGetTransactionLawAPIRequest : AppAPIRequestBase<TermsGetTransactionLawAPIPost, TermsGetTransactionLawAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsGetTransactionLawAPIResponse response ) {
        }
    }
}
