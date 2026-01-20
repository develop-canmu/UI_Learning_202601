using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class AdRewardGenerateOneTimeTokenAPIRequest : AppAPIRequestBase<AdRewardGenerateOneTimeTokenAPIPost, AdRewardGenerateOneTimeTokenAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( AdRewardGenerateOneTimeTokenAPIResponse response ) {
        }
    }
}
