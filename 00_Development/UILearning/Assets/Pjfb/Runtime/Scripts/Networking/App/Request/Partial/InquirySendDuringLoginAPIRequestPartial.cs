using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class InquirySendDuringLoginAPIRequest : AppAPIRequestBase<InquirySendDuringLoginAPIPost, InquirySendDuringLoginAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( InquirySendDuringLoginAPIResponse response ) {
        }
    }
}
