using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaLiberationAPIRequest : AppAPIRequestBase<CharaLiberationAPIPost, CharaLiberationAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaLiberationAPIResponse response ) {
        }
    }
}
