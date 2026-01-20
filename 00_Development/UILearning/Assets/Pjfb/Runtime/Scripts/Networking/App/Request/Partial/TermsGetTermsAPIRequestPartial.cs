using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsGetTermsAPIRequest : AppAPIRequestBase<TermsGetTermsAPIPost, TermsGetTermsAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsGetTermsAPIResponse response ) {
        }
    }
}
