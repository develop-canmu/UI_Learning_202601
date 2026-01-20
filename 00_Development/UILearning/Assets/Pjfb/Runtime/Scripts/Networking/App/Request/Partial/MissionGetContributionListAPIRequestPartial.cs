using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class MissionGetContributionListAPIRequest : AppAPIRequestBase<MissionGetContributionListAPIPost, MissionGetContributionListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( MissionGetContributionListAPIResponse response ) {
        }
    }
}
