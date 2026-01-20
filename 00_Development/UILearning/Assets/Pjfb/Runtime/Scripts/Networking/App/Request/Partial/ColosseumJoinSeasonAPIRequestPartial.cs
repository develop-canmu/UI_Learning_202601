using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumJoinSeasonAPIRequest : AppAPIRequestBase<ColosseumJoinSeasonAPIPost, ColosseumJoinSeasonAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumJoinSeasonAPIResponse response ) {
        }
    }
}
