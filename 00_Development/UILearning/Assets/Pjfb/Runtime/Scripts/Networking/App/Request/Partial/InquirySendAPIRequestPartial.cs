using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class InquirySendAPIRequest : AppAPIRequestBase<InquirySendAPIPost, InquirySendAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( InquirySendAPIResponse response ) {
        }
    }
}
