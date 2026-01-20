using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntGetTimetableDetailAPIRequest : AppAPIRequestBase<HuntGetTimetableDetailAPIPost, HuntGetTimetableDetailAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntGetTimetableDetailAPIResponse response ) {
        }
    }
}
