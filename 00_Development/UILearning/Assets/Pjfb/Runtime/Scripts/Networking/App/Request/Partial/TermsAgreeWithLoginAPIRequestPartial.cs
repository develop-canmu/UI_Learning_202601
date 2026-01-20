using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TermsAgreeWithLoginAPIRequest : AppAPIRequestBase<TermsAgreeWithLoginAPIPost, TermsAgreeWithLoginAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TermsAgreeWithLoginAPIResponse response ) {
        }
    }
}
