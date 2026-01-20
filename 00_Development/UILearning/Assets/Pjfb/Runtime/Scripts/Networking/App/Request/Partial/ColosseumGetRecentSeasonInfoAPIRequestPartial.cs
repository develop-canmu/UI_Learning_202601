using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumGetRecentSeasonInfoAPIRequest : AppAPIRequestBase<ColosseumGetRecentSeasonInfoAPIPost, ColosseumGetRecentSeasonInfoAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumGetRecentSeasonInfoAPIResponse response ) {
        }
    }
}
