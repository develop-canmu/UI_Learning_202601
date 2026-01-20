using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DebugRequirementLevelAssetVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelAssetVerAPIPost, DebugRequirementLevelAssetVerAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DebugRequirementLevelAssetVerAPIResponse response ) {
        }
    }
}
