using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetSeasonStatusAPIRequest : AppAPIRequestBase<ColosseumGetSeasonStatusAPIPost, ColosseumGetSeasonStatusAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetSeasonStatusAPIResponse response ) {
        }
    }
}
