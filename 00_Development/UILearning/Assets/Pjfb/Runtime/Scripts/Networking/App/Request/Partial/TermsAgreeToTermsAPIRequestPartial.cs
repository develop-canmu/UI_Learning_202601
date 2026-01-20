using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsAgreeToTermsAPIRequest : AppAPIRequestBase<TermsAgreeToTermsAPIPost, TermsAgreeToTermsAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsAgreeToTermsAPIResponse response ) {
        }
    }
}
