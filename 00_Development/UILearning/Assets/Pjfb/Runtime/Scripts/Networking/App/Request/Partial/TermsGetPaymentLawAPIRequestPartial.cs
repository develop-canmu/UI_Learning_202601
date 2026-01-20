using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsGetPaymentLawAPIRequest : AppAPIRequestBase<TermsGetPaymentLawAPIPost, TermsGetPaymentLawAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsGetPaymentLawAPIResponse response ) {
        }
    }
}
