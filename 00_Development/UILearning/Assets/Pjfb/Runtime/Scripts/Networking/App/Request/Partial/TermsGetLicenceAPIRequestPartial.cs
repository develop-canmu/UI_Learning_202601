using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsGetLicenceAPIRequest : AppAPIRequestBase<TermsGetLicenceAPIPost, TermsGetLicenceAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsGetLicenceAPIResponse response ) {
        }
    }
}
