using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetUserSeasonStatusPastListAPIRequest : AppAPIRequestBase<ColosseumGetUserSeasonStatusPastListAPIPost, ColosseumGetUserSeasonStatusPastListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetUserSeasonStatusPastListAPIResponse response ) {
        }
    }
}
